using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Confluent.Kafka;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.Kafka;
using NuClear.Tracing.API;

namespace ValidationRules.Hosting.Common
{
    public sealed class KafkaMessageFlowReceiverFactory : IKafkaMessageFlowReceiverFactory
    {
        private readonly ITracer _tracer;
        private readonly IKafkaSettingsFactory _kafkaSettingsFactory;

        public KafkaMessageFlowReceiverFactory(ITracer tracer, IKafkaSettingsFactory kafkaSettingsFactory)
        {
            _tracer = tracer;
            _kafkaSettingsFactory = kafkaSettingsFactory;
        }

        public IKafkaMessageFlowReceiver Create(IMessageFlow messageFlow)
        {
            var settings = _kafkaSettingsFactory.CreateReceiverSettings(messageFlow);
            return new KafkaMessageFlowReceiver2(settings, _tracer);
        }

        // TODO: move to 'messagging' repo after successful testing
        public sealed class KafkaMessageFlowReceiver2 : IKafkaMessageFlowReceiver
        {
            private readonly ConsumerWrapper _consumerWrapper;
            private readonly PollLoop _pollLoop;

            public KafkaMessageFlowReceiver2(IKafkaMessageFlowReceiverSettings settings, ITracer tracer)
            {
                var privateConfig = new Dictionary<string, object>(settings.Config)
                {
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

                var consumer = new Consumer(privateConfig);

                _consumerWrapper = new ConsumerWrapper(consumer, settings.TopicPartitionOffsets, tracer);
                _pollLoop = new PollLoop(consumer, settings.PollTimeout, tracer);
                _pollLoop.StartNew();
            }

            public IReadOnlyCollection<Message> ReceiveBatch(int batchSize)
            {
                var batch = _pollLoop.ReceiveBatch(batchSize);
                return batch;
            }

            public void CompleteBatch(IEnumerable<Message> batch)
            {
                _pollLoop.CompleteBatch(batch);
            }

            public void Dispose()
            {
                _pollLoop.Dispose();
                _consumerWrapper.Dispose();
            }

            private sealed class ConsumerWrapper : IDisposable
            {
                private readonly Consumer _consumer;
                private readonly IEnumerable<TopicPartitionOffset> _topicPartitionOffsets;
                private readonly ITracer _tracer;

                public ConsumerWrapper(Consumer consumer, IEnumerable<TopicPartitionOffset> topicPartitionOffsets, ITracer tracer)
                {
                    _consumer = consumer;
                    _topicPartitionOffsets = topicPartitionOffsets;
                    _tracer = tracer;

                    _consumer.OnError += OnError;
                    _consumer.OnPartitionsAssigned += OnPartitionsAssigned;
                    _consumer.OnPartitionsRevoked += OnPartitionsRevoked;

                    _consumer.Subscribe(_topicPartitionOffsets.Select(x => x.Topic));
                }

                // kafka docs: errors should be seen as informational rather than catastrophic
                private void OnError(object sender, Error error) => _tracer.Warn(error.Reason);

                private void OnPartitionsAssigned(object sender, List<TopicPartition> list)
                {
                    var assignList = list.Select(x =>
                    {
                        var newOffset = _topicPartitionOffsets.Single(y => y.Topic.Equals(x.Topic, StringComparison.OrdinalIgnoreCase)).Offset;
                        return new TopicPartitionOffset(x, newOffset);
                    });

                    ((Consumer)sender).Assign(assignList);
                }

                private static void OnPartitionsRevoked(object sender, List<TopicPartition> list) => ((Consumer)sender).Unassign();

                public void Dispose()
                {
                    _consumer.Unsubscribe();

                    _consumer.OnError -= OnError;
                    _consumer.OnPartitionsAssigned -= OnPartitionsAssigned;
                    _consumer.OnPartitionsRevoked -= OnPartitionsRevoked;

                    _consumer.Dispose();
                }
            }

            // Когда Kafka делает consumer неактивным, то тупо блокируется thread на методе Poll
            // Чтобы от этого не было разных side effects, вынес метод poll в отльный poll loop
            private sealed class PollLoop : IDisposable
            {
                private readonly Consumer _consumer;
                private readonly TimeSpan _pollTimeout;
                private readonly ITracer _tracer;
                private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
                private readonly HashSetLastWins _messages = new HashSetLastWins();

                private Task _task;

                public PollLoop(Consumer consumer, TimeSpan pollTimeout, ITracer tracer)
                {
                    _consumer = consumer;
                    _pollTimeout = pollTimeout;
                    _tracer = tracer;
                }

                public void StartNew()
                {
                    var cancellationToken = _cancellationTokenSource.Token;
                    _task = Task.Factory.StartNew(TaskFunc, cancellationToken, cancellationToken);
                }

                private void TaskFunc(object state)
                {
                    var cancellationToken = (CancellationToken)state;

                    try
                    {
                        _consumer.OnMessage += OnMessage;

                        // loop
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            _consumer.Poll(_pollTimeout);
                        }
                    }
                    catch (Exception ex)
                    {
                        _tracer.Warn(ex, "Kafka audit - exception in poll loop");
                        StartNew();
                    }
                    finally
                    {
                        _consumer.OnMessage -= OnMessage;
                    }

                    void OnMessage(object sender, Message message)
                    {
                        lock (_messages)
                        {
                            _messages.Add(message);
                        }

                        _tracer.Info($"KafkaAudit - fetch message {message.Offset}");
                    }
                }

                public IReadOnlyCollection<Message> ReceiveBatch(int batchSize)
                {
                    lock (_messages)
                    {
                        var batch = _messages.OrderByOffset().Take(batchSize).ToList();
                        _tracer.Info(batch.Count != 0 ? $"KafkaAudit - receive batch [{batch.First().Offset.Value} - {batch.Last().Offset.Value}]" : "KafkaAudit - empty batch");

                        return batch;
                    }
                }

                public void CompleteBatch(IEnumerable<Message> batch)
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    var maxOffsetMessage = batch.OrderBy(x => x.Offset.Value).LastOrDefault();
                    if (maxOffsetMessage == null)
                    {
                        return;
                    }

                    var committedOffsets = _consumer.CommitAsync(maxOffsetMessage).GetAwaiter().GetResult();
                    if (committedOffsets.Error.HasError)
                    {
                        throw new KafkaException(committedOffsets.Error);
                    }

                    var fail = committedOffsets.Offsets.FirstOrDefault(x => x.Error.HasError);
                    if (fail != null)
                    {
                        throw new KafkaException(fail.Error);
                    }

                    lock (_messages)
                    {
                        // ReSharper disable once PossibleMultipleEnumeration
                        _messages.RemoveRange(batch);
                    }

                    foreach (var committedOffset in committedOffsets.Offsets)
                    {
                        _tracer.Info($"KafkaAudit - committed offset {committedOffset.Offset.Value}");
                    }
                }

                public void Dispose()
                {
                    _cancellationTokenSource.Cancel();
                    _task?.Wait();

                    _cancellationTokenSource.Dispose();
                }

                // дедупликация по ключу
                private sealed class HashSetLastWins
                {
                    private readonly Dictionary<byte[], Message> _dictionary = new Dictionary<byte[], Message>(ByteArrayEqualityComparer.Instance);

                    public void Add(Message message) => _dictionary[message.Key] = message;

                    public void RemoveRange(IEnumerable<Message> messages)
                    {
                        foreach (var message in messages)
                        {
                            var existingMessage = _dictionary[message.Key];
                            if (existingMessage.Offset == message.Offset)
                            {
                                _dictionary.Remove(message.Key);
                            }
                        }
                    }

                    public IEnumerable<Message> OrderByOffset() => _dictionary.Values.OrderBy(x => x.Offset.Value);

                    private sealed class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
                    {
                        public static readonly IEqualityComparer<byte[]> Instance = new ByteArrayEqualityComparer();

                        private ByteArrayEqualityComparer() { }

                        public bool Equals(byte[] x, byte[] y)
                        {
                            if (x == y)
                            {
                                return true;
                            }
                            if (x == null || y == null)
                            {
                                return false;
                            }
                            if (x.Length != y.Length)
                            {
                                return false;
                            }

                            for (var i = 0; i < x.Length; i++)
                            {
                                if (!x[i].Equals(y[i]))
                                {
                                    return false;
                                }
                            }

                            return true;
                        }

                        public int GetHashCode(byte[] obj)
                        {
                            var hash = 17;
                            for (var i = 0; i < obj.Length; i++)
                            {
                                hash = hash * 31 + obj[i].GetHashCode();
                            }
                            return hash;
                        }
                    }
                }
            }
        }
    }
}