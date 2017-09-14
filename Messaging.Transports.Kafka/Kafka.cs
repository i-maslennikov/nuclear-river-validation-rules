using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Confluent.Kafka;

using NuClear.Messaging.API.Flows;
using NuClear.Settings.API;
using NuClear.Tracing.API;

namespace NuClear.Messaging.Transports.Kafka
{
    #region move to NuClear.Messaging.Transports.Kafka

    public interface IKafkaMessageFlowReceiver : IDisposable
    {
        IReadOnlyCollection<Message> ReceiveBatch(int batchSize);
        void Complete(Message lastSuccessfulMessage);
    }

    public interface IKafkaMessageFlowReceiverSettings : ISettings
    {
        string ClientId { get; }
        string GroupId { get; }
        Dictionary<string, object> Config { get; }
        IEnumerable<string> Topics { get; }
        TimeSpan PollTimeout { get; }
        Offset Offset { get; }
    }

    public sealed class KafkaMessageFlowReceiver : IKafkaMessageFlowReceiver
    {
        private readonly Consumer _consumer;
        private readonly IKafkaMessageFlowReceiverSettings _settings;
        private readonly ITracer _tracer;

        public KafkaMessageFlowReceiver(IKafkaMessageFlowReceiverSettings settings, ITracer tracer = null)
        {
            _settings = settings;
            _tracer = tracer;

            var privateConfig = new Dictionary<string, object>(_settings.Config)
                {
                    // client.id used in kafka monitoring
                    { "client.id", _settings.ClientId },

                    { "group.id", _settings.GroupId },

                    // performance tricks from denis
                    { "socket.blocking.max.ms", 1 },
                    { "fetch.wait.max.ms", 5 },
                    { "fetch.error.backoff.ms", 5 },
                    { "fetch.message.max.bytes", 10240 },
                    { "queued.min.messages", 1000 },

                    // manual commit
                    { "enable.auto.commit", false },

                    // reset to minimum offset
                    { "default.topic.config", new Dictionary<string, object>
                            {
                                { "auto.offset.reset", "smallest" }
                            }
                    },

                    // check compatible kafka version
                    { "api.version.request", true },
                };

            _consumer = new Consumer(privateConfig);
            _consumer.OnError += OnError;
            _consumer.Assign(_settings.Topics.Select(x => new TopicPartitionOffset(x, 0, _settings.Offset)));
        }

        public IReadOnlyCollection<Message> ReceiveBatch(int batchSize)
        {
            var messages = new HashSetLastWins();
            var eof = false;
            Message errorMessage = null;

            try
            {
                _consumer.OnMessage += OnMessage;
                _consumer.OnConsumeError += OnErrorMessage;
                _consumer.OnPartitionEOF += OnPartitionEof;

                var sw = Stopwatch.StartNew();
                while (true)
                {
                    if (messages.Count >= batchSize)
                    {
                        break;
                    }

                    if (errorMessage != null)
                    {
                        break;
                    }

                    if (eof)
                    {
                        break;
                    }

                    var timeLeft = _settings.PollTimeout - sw.Elapsed;
                    if (timeLeft.Ticks <= 0)
                    {
                        break;
                    }

                    _consumer.Poll(timeLeft);
                }

                if (errorMessage != null)
                {
                    throw new KafkaException(errorMessage.Error);
                }

                return messages.ToReadOlnyCollection();
            }
            finally
            {
                _consumer.OnMessage -= OnMessage;
                _consumer.OnConsumeError -= OnErrorMessage;
                _consumer.OnPartitionEOF -= OnPartitionEof;
            }

            void OnMessage(object sender, Message message) => messages.Add(message);
            void OnErrorMessage(object sender, Message message) => errorMessage = message;
            void OnPartitionEof(object sender, TopicPartitionOffset offset) => eof = true;
        }

        public void Complete(Message lastSuccessfulMessage)
        {
            var committedOffsets = _consumer.CommitAsync(lastSuccessfulMessage).GetAwaiter().GetResult();

            if (committedOffsets.Error.HasError)
            {
                throw new KafkaException(committedOffsets.Error);
            }
        }

        // kafka docs: errors should be seen as informational rather than catastrophic
        private void OnError(object sender, Error error) => _tracer?.Warn(error.Reason);

        public void Dispose()
        {
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

            private sealed class ByteArrayEqualityComparer: IEqualityComparer<byte[]>
            {
                public static readonly IEqualityComparer<byte[]> Instance = new ByteArrayEqualityComparer();

                private ByteArrayEqualityComparer() { }

                public bool Equals(byte[] x, byte[] y)
                {
                    return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
                }

                public int GetHashCode(byte[] obj)
                {
                    return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
                }
            }
        }
    }

    public interface IKafkaMessageFlowReceiverFactory
    {
        IKafkaMessageFlowReceiver Create(IMessageFlow messageFlow);
    }

    #endregion
}
