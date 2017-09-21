using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Confluent.Kafka;

using Newtonsoft.Json;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.Kafka;
using NuClear.River.Hosting.Common.Settings;
using NuClear.Settings;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Tracing.API;
using NuClear.ValidationRules.OperationsProcessing.Transports.Kafka;
using NuClear.ValidationRules.Replication.Dto;

using ValidationRules.Hosting.Common;

namespace NuClear.ValidationRules.StateInitialization.Host
{
    internal sealed class AmsStateInit
    {
        public static IEqualityComparer<AdvertisementDto> IdComparer { get; } = new IdEqualityComparer();

        private readonly IKafkaMessageFlowReceiverFactory _receiverFactory;
        private readonly AmsBatchSizeSettings _batchSizeSettings = new AmsBatchSizeSettings();

        public AmsStateInit(IConnectionStringSettings connectionStringSettings)
        {
            var amsSettingsFactory = new AmsSettingsFactory(connectionStringSettings, new EnvironmentSettingsAspect(), Offset.Beginning);
            _receiverFactory = new KafkaMessageFlowReceiverFactory(new NullTracer(), amsSettingsFactory);
        }

        public IEnumerable<object> GeReplicationDtos(IMessageFlow messageFlow)
        {
            var hashSet = new HashSet<AdvertisementDto>(IdComparer);

            using (var receiver = _receiverFactory.Create(messageFlow))
            {
                IReadOnlyCollection<Message> batch;
                while ((batch = receiver.ReceiveBatch(_batchSizeSettings.BatchSize)).Count != 0)
                {
                    var maxOffsetMesasage = batch.OrderByDescending(x => x.Offset.Value).First();
                    Console.WriteLine($"Received {batch.Count} messages, offset {maxOffsetMesasage.Offset}");

                    var dtos = batch
                        .Where(x => x.Value != null)
                        .Select(x => JsonConvert.DeserializeObject<AdvertisementDto>(Encoding.UTF8.GetString(x.Value))).ToList();

                    hashSet.UnionWith(dtos);

                    receiver.CompleteBatch(batch);

                    // state init имеет смысл прекращать когда мы вычитали все полные батчи
                    // а то нам могут до бесконечности подкидывать новых messages
                    if (batch.Count != _batchSizeSettings.BatchSize)
                    {
                        break;
                    }
                }
            }

            foreach (var dto in hashSet)
            {
                yield return dto;
            }
        }

        // уникальность внутри одного batch гарантируется клиентом kafka
        // уникальность между всеми batch надо делать самим
        private sealed class IdEqualityComparer : IEqualityComparer<AdvertisementDto>
        {
            public bool Equals(AdvertisementDto x, AdvertisementDto y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Id == y.Id;
            }

            public int GetHashCode(AdvertisementDto obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        private sealed class AmsBatchSizeSettings
        {
            public int BatchSize { get; } = ConfigFileSetting.Int.Optional("AmsBatchSize", 5000).Value;
        }
    }
}
