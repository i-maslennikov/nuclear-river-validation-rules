using System;
using System.Collections.Generic;

using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core;
using NuClear.ValidationRules.OperationsProcessing;
using NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow;
using NuClear.ValidationRules.Replication.Dto;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.StateInitialization.Host.Kafka.Ams
{
    public sealed class AmsFactsBulkCommandFactory : IBulkCommandFactory<Confluent.Kafka.Message>
    {
        private readonly IDeserializer<Confluent.Kafka.Message, AdvertisementDto> _deserializer;

        public AmsFactsBulkCommandFactory()
        {
            _deserializer = new AdvertisementDtoDeserializer();
            AppropriateFlows = new[] { AmsFactsFlow.Instance };
        }

        public IReadOnlyCollection<IMessageFlow> AppropriateFlows { get; }

        public IReadOnlyCollection<ICommand> CreateCommands(Confluent.Kafka.Message kafkaMessage)
        {
            var dtos = _deserializer.Deserialize(kafkaMessage);
            if (dtos.Count == 0)
            {
                return Array.Empty<ICommand>();
            }

            return new[]
                {
                    new KafkaReplicationActor.BulkInsertDataObjectsCommand(typeof(Advertisement), dtos),
                    new KafkaReplicationActor.BulkInsertDataObjectsCommand(typeof(EntityName), dtos)
                };
        }
    }
}
