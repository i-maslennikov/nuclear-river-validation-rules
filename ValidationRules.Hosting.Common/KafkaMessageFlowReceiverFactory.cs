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

            public KafkaMessageFlowReceiver2(IKafkaMessageFlowReceiverSettings settings)
                : this(settings, null) { }

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
                _pollLoop = new PollLoop(consumer, settings.PollTimeout, tracer).StartNew();
            }

            public IReadOnlyCollection<Message> ReceiveBatch(int batchSize)
            {
                var batch = _pollLoop.ReceiveBatch(batchSize);
                return batch;
            }

            public void CompleteBatch(IEnumerable<Message> batch)
            {
                _consumerWrapper.CompleteBatch(batch);
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

                public void CompleteBatch(IEnumerable<Message> batch)
                {
                    var maxOffsetMessage = batch.OrderByDescending(x => x.Offset.Value).FirstOrDefault();
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

                    foreach (var committedOffset in committedOffsets.Offsets)
                    {
                        _tracer?.Debug($"KafkaAudit - committed offset {committedOffset.Offset.Value}");
                    }
                }

                // kafka docs: errors should be seen as informational rather than catastrophic
                private void OnError(object sender, Error error) => _tracer?.Warn(error.Reason);

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
                private static readonly object SyncRoot = new object();

                private readonly Consumer _consumer;
                private readonly TimeSpan _pollTimeout;
                private readonly ITracer _tracer;
                private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
                private readonly HashSetLastWins _messages = new HashSetLastWins();

                public PollLoop(Consumer consumer, TimeSpan pollTimeout, ITracer tracer)
                {
                    _consumer = consumer;
                    _pollTimeout = pollTimeout;
                    _tracer = tracer;
                }

                public PollLoop StartNew()
                {
                    var cancellationToken = _cancellationTokenSource.Token;

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            _consumer.OnMessage += OnMessage;

                            // loop
                            while (!cancellationToken.IsCancellationRequested)
                            {
                                _consumer.Poll(_pollTimeout);
                            }
                        }
                        catch(Exception ex)
                        {
                            _tracer?.Warn(ex, "Kafka audit - exception in poll loop");
                            StartNew();
                        }
                        finally
                        {
                            _consumer.OnMessage -= OnMessage;
                        }

                        void OnMessage(object sender, Message message)
                        {
                            lock (SyncRoot)
                            {
                                _messages.Add(message);
                            }

                            _tracer.Debug($"KafkaAudit - fetch message {message.Offset}");
                        }
                    }, cancellationToken);

                    return this;
                }

                public IReadOnlyCollection<Message> ReceiveBatch(int batchSize)
                {
                    lock (SyncRoot)
                    {
                        var batch = _messages.OrderByOffset().Take(batchSize).ToList();
                        if (batch.Count != 0)
                        {
                            _messages.RemoveRange(batch);

                            _tracer?.Debug($"KafkaAudit - receive batch [{batch.First().Offset.Value} - {batch.Last().Offset.Value}]");
                        }
                        else
                        {
                            _tracer?.Debug("KafkaAudit - empty batch");
                        }

                        return batch;
                    }
                }

                public void Dispose()
                {
                    _cancellationTokenSource.Cancel();
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
                            _dictionary.Remove(message.Key);
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