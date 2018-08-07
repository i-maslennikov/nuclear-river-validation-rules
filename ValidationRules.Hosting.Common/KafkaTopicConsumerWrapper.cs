using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Confluent.Kafka;
using NuClear.Messaging.Transports.Kafka;
using NuClear.Tracing.API;

namespace ValidationRules.Hosting.Common
{
    // TODO: move to 'messagging' repo after successful testing
    public sealed class KafkaTopicConsumerWrapper : IKafkaMessageFlowReceiver
    {
        private readonly Consumer _consumer;
        private readonly IReadOnlyCollection<TopicPartitionOffset> _topicPartitionOffsets;
        private readonly TimeSpan _pollTimeout;
        private readonly ITracer _tracer;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly CompactedKafkaMessageQueue _messageQueue = new CompactedKafkaMessageQueue();
        private readonly Task _task;

        private readonly Action _backpressureBehaviour;

        public KafkaTopicConsumerWrapper(IKafkaMessageFlowReceiverSettings settings, ITracer tracer)
        {
            var privateConfig = new Dictionary<string, object>(settings.Config)
                {
                    // help kafka server logs to identify node
                    { "client.id", Environment.MachineName },

                    // manual commit
                    { "enable.auto.commit", false },

                    {
                        "default.topic.config", new Dictionary<string, object>
                            {
                                // reset to minimum offset
                                { "auto.offset.reset", "smallest" }
                            }
                    },

                    // check compatible kafka version
                    { "api.version.request", true },
                };

            _topicPartitionOffsets = settings.TopicPartitionOffsets
                                             .ToList();
            _pollTimeout = settings.PollTimeout;
            _tracer = tracer;

            _consumer = new Consumer(privateConfig);

            _backpressureBehaviour = privateConfig.TryGetValue("queued.max.messages.kbytes", out var rawQueuedMaxKb)
                                         ? (Action)new MaxQueuedKbBackpressureBehaviour(int.Parse((string)rawQueuedMaxKb),
                                                                                        _topicPartitionOffsets.Select(x => new TopicPartition(x.Topic, x.Partition)),
                                                                                        _consumer,
                                                                                        _messageQueue,
                                                                                        tracer)
                                             .Execute
                                         : () => { /* nop backpressure behavior */};

            _consumer.OnError += OnError;
            _consumer.OnPartitionsAssigned += OnPartitionsAssigned;
            _consumer.OnPartitionsRevoked += OnPartitionsRevoked;
            _consumer.Subscribe(_topicPartitionOffsets.Select(x => x.Topic));

            _tracer.Info("KafkaAudit. Topic consumer created");

            _task = Task.Factory.StartNew(PollFunc,
                                          _cancellationTokenSource.Token,
                                          _cancellationTokenSource.Token,
                                          TaskCreationOptions.LongRunning,
                                          TaskScheduler.Default);

            _tracer.Info("KafkaAudit. Topic consumer started");
        }

        IReadOnlyCollection<Message> IKafkaMessageFlowReceiver.ReceiveBatch(int batchSize)
        {
            var batch = _messageQueue.PeekOrderedMessageBatch(batchSize);
            _tracer.Info(batch.Count != 0
                             ? $"KafkaAudit - receive batch {batch.First().TopicPartition} @{batch.First().Offset.Value}-{batch.Last().Offset.Value}"
                             : "KafkaAudit - empty batch");

            return batch;
        }

        void IKafkaMessageFlowReceiver.CompleteBatch(IEnumerable<Message> messages)
        {
            var partitions2LastMessages = messages.GroupBy(x => x.TopicPartition)
                                                  .Select(group => new
                                                      {
                                                          group.Key,
                                                          MaxOffsetMessage = group.OrderBy(x => x.Offset.Value).LastOrDefault()
                                                      })
                                                  .Where(x => x.MaxOffsetMessage != null)
                                                  .ToDictionary(x => x.Key, x => x.MaxOffsetMessage);
            if (!partitions2LastMessages.Any())
            {
                return;
            }

            var unassignedPartitionsCommitAttempt = partitions2LastMessages.Keys
                                                                           .Except(_consumer.Assignment)
                                                                           .ToList();
            if (unassignedPartitionsCommitAttempt.Any())
            {
                var errorMessage = $"Attempt to commit offsets for not assigned partitions: {string.Join(";", unassignedPartitionsCommitAttempt)}";
                _tracer.Error(errorMessage);
                throw new KafkaException(new Error(ErrorCode.Unknown, errorMessage));
            }

            var committedOffsets = _consumer.CommitAsync(partitions2LastMessages.Values.Select(x => x.TopicPartitionOffset))
                                            .GetAwaiter()
                                            .GetResult();
            if (committedOffsets.Error.HasError)
            {
                _tracer.Warn($"KafkaAudit - error occured while committing offsets {committedOffsets.Error}");
                throw new KafkaException(committedOffsets.Error);
            }

            var failOffset = committedOffsets.Offsets.FirstOrDefault(x => x.Error.HasError);
            if (failOffset != null)
            {
                _tracer.Warn($"KafkaAudit - error occured while committing offset {failOffset}");
                throw new KafkaException(failOffset.Error);
            }

            // ReSharper disable once PossibleMultipleEnumeration
            _messageQueue.Remove(messages);

            // logging
            foreach (var committedOffset in committedOffsets.Offsets)
            {
                _tracer.Info($"KafkaAudit - committed offset {committedOffset}");
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();

            try
            {
                _task?.Wait();
            }
            catch (AggregateException ae)
            {
                ae.Handle(e => e is OperationCanceledException);
            }

            _consumer.Unsubscribe();

            _consumer.OnPartitionsAssigned -= OnPartitionsAssigned;
            _consumer.OnPartitionsRevoked -= OnPartitionsRevoked;
            _consumer.OnError -= OnError;

            _cancellationTokenSource.Dispose();
            _consumer.Dispose();

            _tracer.Info("KafkaAudit - poll loop disposed");
        }

        /// <summary>
        /// Когда Kafka делает consumer неактивным, то тупо блокируется thread на методе Consumer.Poll/Consume
        /// Чтобы от этого не было разных side effects, используем отдельный цикл для опроса, выполняемый из независимого (от клиентов данного типа) потока
        /// </summary>
        private void PollFunc(object state)
        {
            var cancellationToken = (CancellationToken)state;

            // retryable poll loop
            while (!cancellationToken.IsCancellationRequested)
            {
                _backpressureBehaviour.Invoke();

                try
                {
                    if (!_consumer.Consume(out var message, _pollTimeout))
                    {
                        continue;
                    }

                    _messageQueue.Enqueue(message);
                    _tracer.Info($"KafkaAudit - fetch message {message.TopicPartitionOffset}");
                }
                catch (Exception ex)
                {
                    _tracer.Warn(ex, "KafkaAudit - error in poll loop, retrying");
                }
            }
        }

        private void OnPartitionsAssigned(object _, List<TopicPartition> list)
        {
            var assignList = list.Select(x =>
                                             {
                                                 var newOffset = _topicPartitionOffsets.Single(y => y.Topic.Equals(x.Topic, StringComparison.OrdinalIgnoreCase)).Offset;
                                                 return new TopicPartitionOffset(x, newOffset);
                                             })
                                 .ToList();

            _consumer.Assign(assignList);

            _tracer.Info($"KafkaAudit - assigned partitions: {string.Join(",", assignList)}");
        }

        private void OnPartitionsRevoked(object _, List<TopicPartition> list)
        {
            _consumer.Unassign();
            _messageQueue.Clear();

            _tracer.Info($"KafkaAudit - revoked partitions: {string.Join(", ", list)}");
        }

        // kafka docs: errors should be seen as informational rather than catastrophic
        private void OnError(object _, Error error) => _tracer.Warn($"KafkaAudit - error {error.Reason}");

        private sealed class MaxQueuedKbBackpressureBehaviour
        {
            private readonly int _maxQueuedKbThreshold;
            private readonly IEnumerable<TopicPartition> _targetPartitions;
            private readonly Consumer _consumer;
            private readonly CompactedKafkaMessageQueue _messageQueue;
            private readonly ITracer _tracer;

            private bool _isBackpressureMode;

            public MaxQueuedKbBackpressureBehaviour(
                int maxQueuedKbThreshold,
                IEnumerable<TopicPartition> targetPartitions,
                Consumer consumer,
                CompactedKafkaMessageQueue messageQueue,
                ITracer tracer)
            {
                _maxQueuedKbThreshold = maxQueuedKbThreshold;
                _targetPartitions = targetPartitions;
                _consumer = consumer;
                _messageQueue = messageQueue;
                _tracer = tracer;
            }

            public void Execute()
            {
                var actualQueuedKb = _messageQueue.ActualQueuedKb;
                if (_maxQueuedKbThreshold <= actualQueuedKb)
                {
                    if (_isBackpressureMode)
                    {
                        return;
                    }

                    _tracer.Debug($"KafkaAudit. Backpressure mode. Limit for messages in queue is {_maxQueuedKbThreshold} Kb. Actual queued {actualQueuedKb} Kb");

                    var pausedPartitions = _consumer.Pause(_targetPartitions);
                    foreach (var partition in pausedPartitions)
                    {
                        _tracer.Debug($"{partition.TopicPartition} pause attempt finished with result {partition.Error}");
                    }
                    _isBackpressureMode = true;
                }
                else if (_isBackpressureMode)
                {
                    var resumedPartitions = _consumer.Resume(_targetPartitions);
                    foreach (var partition in resumedPartitions)
                    {
                        _tracer.Debug($"{partition.TopicPartition} resume attempt finished with result {partition.Error}");
                    }

                    _isBackpressureMode = false;
                }
            }
        }
    }
}