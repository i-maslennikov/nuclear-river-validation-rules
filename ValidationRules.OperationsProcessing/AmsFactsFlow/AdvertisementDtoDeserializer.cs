using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using NuClear.ValidationRules.Replication.Dto;

namespace NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow
{
    public sealed class AdvertisementDtoDeserializer : IDeserializer<Confluent.Kafka.Message, AdvertisementDto>
    {
        public IReadOnlyCollection<AdvertisementDto> Deserialize(Confluent.Kafka.Message kafkaMessage)
        {
            // filter heartbeat & tombstone messages
            var messagePayload = kafkaMessage.Value;
            if (messagePayload == null)
            {
                return Array.Empty<AdvertisementDto>();
            }

            var dto = JsonConvert.DeserializeObject<AdvertisementDto>(Encoding.UTF8.GetString(messagePayload));
            dto.Offset = kafkaMessage.Offset;

            return new[] { dto };
        }
    }
}