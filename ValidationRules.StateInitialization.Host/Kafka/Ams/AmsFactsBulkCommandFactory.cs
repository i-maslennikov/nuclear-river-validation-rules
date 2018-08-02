using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core;
using NuClear.ValidationRules.OperationsProcessing;
using NuClear.ValidationRules.OperationsProcessing.Facts.AmsFactsFlow;
using NuClear.ValidationRules.Replication.Dto;

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

        public IReadOnlyCollection<ICommand> CreateCommands(IReadOnlyCollection<Confluent.Kafka.Message> messages)
        {
            var deserializedDtos = messages.AsParallel()
                                           .Select(message => _deserializer.Deserialize(message))
                                           .AsSequential()
                                           .Aggregate(new List<AdvertisementDto>(messages.Count),
                                                      (dtos, collection) =>
                                                          {
                                                              dtos.AddRange(collection);
                                                              return dtos;
                                                          });

            if (deserializedDtos.Count == 0)
            {
                return Array.Empty<ICommand>();
            }

            return DataObjectTypesProviderFactory.AmsFactTypes
                                                 .Select(factType => new KafkaReplicationActor.BulkInsertDataObjectsCommand(factType, deserializedDtos))
                                                 .ToList();
        }
    }
}
