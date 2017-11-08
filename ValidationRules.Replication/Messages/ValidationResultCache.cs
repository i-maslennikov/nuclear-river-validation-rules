using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Newtonsoft.Json;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Writings;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Messages
{
    public sealed class ValidationResultCache
    {
        private readonly IQuery _query;
        private readonly IRepository<Cache.ValidationResult> _repository;

        public ValidationResultCache(IQuery query, IRepository<Cache.ValidationResult> repository)
        {
            _query = query;
            _repository = repository;
        }

        public bool TryGet(MessageTypeCode messageType, out IReadOnlyCollection<Version.ValidationResult> value)
        {
            var cachedResult = _query.For<Cache.ValidationResult>().SingleOrDefault(x => x.MessageType == messageType);

            if (cachedResult == null)
            {
                value = null;
                return false;
            }

            value = Deserialize(cachedResult.Data);
            return true;
        }

        public void Initialize(MessageTypeCode messageType, IReadOnlyCollection<Version.ValidationResult> value)
        {
            var result = new Cache.ValidationResult { MessageType = messageType, Data = Serialize(value) };
            _repository.Add(result);
            _repository.Save();
        }

        public void Update(MessageTypeCode messageType, IReadOnlyCollection<Version.ValidationResult> value)
        {
            var result = new Cache.ValidationResult { MessageType = messageType, Data = Serialize(value) };
            _repository.Update(result);
            _repository.Save();
        }

        private IReadOnlyCollection<Version.ValidationResult> Deserialize(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var stream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<IReadOnlyCollection<Version.ValidationResult>>(jsonReader);
            }
        }

        private byte[] Serialize(IReadOnlyCollection<Version.ValidationResult> value)
        {
            using (var compressedStream = new MemoryStream())
            {
                using (var stream = new DeflateStream(compressedStream, CompressionMode.Compress))
                using (var writer = new StreamWriter(stream))
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(jsonWriter, value);
                }

                return compressedStream.ToArray();
            }
        }
    }
}