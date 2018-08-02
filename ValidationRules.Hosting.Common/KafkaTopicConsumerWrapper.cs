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
        private readonly IEnumerable<TopicPartitionOffset> _topicPartitionOffsets;
        private readonly TimeSpan _pollTimeout;
        private readonly int? _maybeBackpressureQueuedMaxKb;
        private readonly ITracer _tracer;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly CompactedKafkaMessageQueue _messageQueue = new CompactedKafkaMessageQueue();
        private readonly Task _task;

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


            var maybeBackpressureQueuedMaxKb = privateConfig.TryGetValue("queued.max.messages.kbytes", out var rawQueuedMaxKb)
                                                   ? int.Parse((string)rawQueuedMaxKb)
                                                   : (int?)null;

            _consumer = new Consumer(privateConfig);

            _topicPartitionOffsets = settings.TopicPartitionOffsets;
            _pollTimeout = settings.PollTimeout;
            _maybeBackpressureQueuedMaxKb = maybeBackpressureQueuedMaxKb;
            _tracer = tracer;

            _consumer.OnMessage += OnMessage;
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
            // ReSharper disable once PossibleMultipleEnumeration
            var maxOffsetMessage = messages.OrderBy(x => x.Offset.Value).LastOrDefault();
            if (maxOffsetMessage == null)
            {
                return;
            }

            var committedOffsets = _consumer.CommitAsync(maxOffsetMessage).GetAwaiter().GetResult();
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
            _consumer.OnMessage -= OnMessage;

            _cancellationTokenSource.Dispose();
            _consumer.Dispose();

            _tracer.Info("KafkaAudit - poll loop disposed");
        }

        /// <summary>
        /// Когда Kafka делает consumer неактивным, то тупо блокируется thread на методе Consumer.Poll
        /// Чтобы от этого не было разных side effects, используем отдельный цикл для опроса, выполняемый из независимого (от клиентов данного типа) потока
        /// </summary>
        private void PollFunc(object state)
        {
            var cancellationToken = (CancellationToken)state;

            // retryable poll loop
            while (!cancellationToken.IsCancellationRequested)
            {
                var actualQueuedKb = _messageQueue.ActualQueuedKb;
                if (_maybeBackpressureQueuedMaxKb.HasValue
                    && _maybeBackpressureQueuedMaxKb.Value <= actualQueuedKb)
                {
                    _tracer.Debug($"KafkaAudit. Backpressure mode. Limit for messages in queue is {_maybeBackpressureQueuedMaxKb.Value} Kb. Actual queued {actualQueuedKb} Kb");
                    Thread.Sleep(1000);
                    continue;
                }

                try
                {
                    _consumer.Poll(_pollTimeout);
                }
                catch (Exception ex)
                {
                    _tracer.Warn(ex, "KafkaAudit - error in poll loop, retrying");
                }
            }
        }

        private void OnMessage(object _, Message message)
        {
            _messageQueue.Enqueue(message);

            _tracer.Info($"KafkaAudit - fetch message {message.TopicPartitionOffset}");
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

            _tracer.Info($"KafkaAudit - revoked partitions: {string.Join(", ", list)}");
        }

        // kafka docs: errors should be seen as informational rather than catastrophic
        private void OnError(object _, Error error) => _tracer.Warn($"KafkaAudit - error {error.Reason}");
    }
}