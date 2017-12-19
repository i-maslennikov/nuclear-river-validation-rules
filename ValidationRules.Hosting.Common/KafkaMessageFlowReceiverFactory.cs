using System.Collections.Generic;
using System.Linq;

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

        // TODO: copy-paster from messagging, remove after tests
        public sealed class KafkaMessageFlowReceiver2 : IKafkaMessageFlowReceiver
        {
            private readonly Consumer _consumer;
            private readonly IKafkaMessageFlowReceiverSettings _settings;
            private readonly ITracer _tracer;

            public KafkaMessageFlowReceiver2(IKafkaMessageFlowReceiverSettings settings)
                : this(settings, null)
            {
            }

            public KafkaMessageFlowReceiver2(IKafkaMessageFlowReceiverSettings settings, ITracer tracer)
            {
                _settings = settings;
                _tracer = tracer;

                var privateConfig = new Dictionary<string, object>(_settings.Config)
                    {
                        // manual commit
                        { "enable.auto.commit", false },

                        // reset to minimum offset
                        {
                            "default.topic.config", new Dictionary<string, object>
                                {
                                    { "auto.offset.reset", "smallest" }
                                }
                        },

                        // check compatible kafka version
                        { "api.version.request", true },
                    };

                _consumer = new Consumer(privateConfig);
                _consumer.OnError += OnError;
                _consumer.Assign(_settings.TopicPartitionOffsets);
            }

            static long prevMaxOffset;

            public IReadOnlyCollection<Message> ReceiveBatch(int batchSize)
            {
                var minOffset = long.MaxValue;
                var maxOffset = long.MinValue;

                var messages = new HashSetLastWins();
                var eof = false;
                Message errorMessage = null;

                try
                {
                    _consumer.OnMessage += OnMessage;
                    _consumer.OnConsumeError += OnErrorMessage;
                    _consumer.OnPartitionEOF += OnPartitionEof;

                    _consumer.Poll(_settings.PollTimeout);
                    while (true)
                    {
                        if (errorMessage != null)
                        {
                            throw new KafkaException(errorMessage.Error);
                        }

                        if (messages.Count >= batchSize)
                        {
                            break;
                        }

                        if (eof)
                        {
                            break;
                        }

                        _consumer.Poll(_settings.PollTimeout);
                    }

                    var values = messages.ToReadOlnyCollection();

                    // TODO: remove tracing
                    if (minOffset != long.MaxValue)
                    {
                        _tracer.Info($"KafkaAudit - receive batch [{minOffset}-{maxOffset}]");

                        if (prevMaxOffset != 0 && minOffset != prevMaxOffset + 1)
                        {
                            _tracer.Error($"KafkaAudit - error minOffset {minOffset} prevMaxOffset {prevMaxOffset}");
                        }
                        prevMaxOffset = maxOffset;
                    }
                    else
                    {
                        _tracer.Info("KafkaAudit - receive empty batch");
                    }

                    return values;
                }
                finally
                {
                    _consumer.OnMessage -= OnMessage;
                    _consumer.OnConsumeError -= OnErrorMessage;
                    _consumer.OnPartitionEOF -= OnPartitionEof;
                }

                void OnMessage(object sender, Message message)
                {
                    var offset = message.Offset.Value;
                    if (offset < minOffset)
                    {
                        minOffset = offset;
                    }
                    if (offset > maxOffset)
                    {
                        maxOffset = offset;
                    }

                    messages.Add(message);
                }

                void OnErrorMessage(object sender, Message message) => errorMessage = message;
                void OnPartitionEof(object sender, TopicPartitionOffset offset) => eof = true;
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
            }

            // kafka docs: errors should be seen as informational rather than catastrophic
            private void OnError(object sender, Error error) => _tracer?.Warn(error.Reason);

            public void Dispose()
            {
                _consumer.Unassign();
                _consumer.OnError -= OnError;
                _consumer.Dispose();
            }

            // дедупликация по ключу
            private sealed class HashSetLastWins
            {
                private readonly Dictionary<byte[], Message> _dictionary = new Dictionary<byte[], Message>(ByteArrayEqualityComparer.Instance);

                public void Add(Message message) => _dictionary[message.Key] = message;

                public int Count => _dictionary.Count;
                public IReadOnlyCollection<Message> ToReadOlnyCollection() => _dictionary.Values;

                private sealed class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
                {
                    public static readonly IEqualityComparer<byte[]> Instance = new ByteArrayEqualityComparer();

                    private ByteArrayEqualityComparer()
                    {
                    }

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