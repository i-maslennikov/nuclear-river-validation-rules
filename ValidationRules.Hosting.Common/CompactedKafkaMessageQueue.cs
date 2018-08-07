using System.Collections.Generic;
using System.Linq;

using Confluent.Kafka;

namespace ValidationRules.Hosting.Common
{
    public sealed class CompactedKafkaMessageQueue
    {
        private readonly Dictionary<byte[], Message> _messageKey2LatestPayloadMap = new Dictionary<byte[], Message>(ByteArrayEqualityComparer.Instance);

        public int ActualQueuedKb { get; private set; }

        public void Enqueue(Message message)
        {
            lock (_messageKey2LatestPayloadMap)
            {
                if (_messageKey2LatestPayloadMap.TryGetValue(message.Key, out var existingMessage))
                {
                    ActualQueuedKb -= SizeOfInKb(existingMessage);
                }

                _messageKey2LatestPayloadMap[message.Key] = message;
                ActualQueuedKb += SizeOfInKb(message);
            }
        }

        public void Remove(IEnumerable<Message> messages)
        {
            lock (_messageKey2LatestPayloadMap)
            {
                foreach (var message in messages)
                {
                    var existingMessage = _messageKey2LatestPayloadMap[message.Key];
                    if (existingMessage.Offset == message.Offset)
                    {
                        _messageKey2LatestPayloadMap.Remove(message.Key);
                        ActualQueuedKb -= SizeOfInKb(message);
                    }
                }
            }
        }

        public void Clear()
        {
            lock (_messageKey2LatestPayloadMap)
            {
                _messageKey2LatestPayloadMap.Clear();
                ActualQueuedKb = 0;
            }
        }

        public IReadOnlyList<Message> PeekOrderedMessageBatch(int batchSize)
        {
            lock (_messageKey2LatestPayloadMap)
            {
                return _messageKey2LatestPayloadMap.Values
                                  .OrderBy(x => x.Offset.Value)
                                  .Take(batchSize)
                                  .ToList();
            }
        }

        private int SizeOfInKb(Message message)
        {
            var keySize = message.Key?.Length ?? 0;
            var payloadSize = message.Value?.Length ?? 0;
            return (keySize + payloadSize) / 1024;
        }

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