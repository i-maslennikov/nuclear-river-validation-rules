using System;
using System.Collections;
using System.Collections.Generic;
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

            _consumer.OnPartitionsAssigned += OnPartitionsAssigned;
            _consumer.OnPartitionsRevoked += OnPartitionsRevoked;
            _consumer.OnError += OnError;

            _consumer.Subscribe(_settings.Topics);
        }

        public IReadOnlyCollection<Message> ReceiveBatch(int batchSize)
        {
            var currentMessage = (Message)null;
            var eof = false;
            var idleCount = 2;

            try
            {
                _consumer.OnMessage += OnMessage;
                _consumer.OnConsumeError += OnMessage;
                _consumer.OnPartitionEOF += OnPartitionEof;

                var messages = new HashSetLastWins();
                while (messages.Count < batchSize)
                {
                    currentMessage = null;
                    // Таймаут действует для каждого одного сообщения. Если в очередь будут добавляться сообщения с интервалом 4 секунды, то пройдёт полчаса, прежде чем пакет будет принят.
                    _consumer.Poll(_settings.PollTimeout);

                    if (eof)
                    {
                        break;
                    }

                    if (currentMessage == null)
                    {
                        // иногда Poll вообще ничего не возвращает, тогда надо ещё раз вызвать Poll
                        // может быть я неправильно готовлю, но пока вот такой вот workaround
                        if (idleCount != 0)
                        {
                            idleCount--;
                            continue;
                        }

                        break;
                    }

                    if (currentMessage.Error.HasError)
                    {
                        throw new KafkaException(currentMessage.Error);
                    }

                    messages.Add(currentMessage);
                }

                return messages.ToReadOlnyCollection();
            }
            finally
            {
                _consumer.OnMessage -= OnMessage;
                _consumer.OnConsumeError -= OnMessage;
                _consumer.OnPartitionEOF -= OnPartitionEof;
            }

            void OnMessage(object sender, Message message) => currentMessage = message;
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

        private void OnPartitionsAssigned(object sender, List<TopicPartition> partitions)
        {
            var offests = partitions.Select(x => new TopicPartitionOffset(x, _settings.Offset)).ToList();
            ((Consumer)sender).Assign(offests);
        }

        private static void OnPartitionsRevoked(object sender, List<TopicPartition> partitions) => ((Consumer)sender).Unassign();

        // kafka docs: errors should be seen as informational rather than catastrophic
        private void OnError(object sender, Error error) => _tracer?.Warn(error.Reason);

        public void Dispose()
        {
            _consumer.Unsubscribe();

            _consumer.OnPartitionsAssigned -= OnPartitionsAssigned;
            _consumer.OnPartitionsRevoked -= OnPartitionsRevoked;
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
