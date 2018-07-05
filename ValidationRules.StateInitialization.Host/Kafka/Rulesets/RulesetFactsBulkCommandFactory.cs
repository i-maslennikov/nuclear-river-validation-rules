using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core;
using NuClear.ValidationRules.OperationsProcessing;
using NuClear.ValidationRules.OperationsProcessing.RulesetFactsFlow;
using NuClear.ValidationRules.Replication.Dto;
using NuClear.ValidationRules.Storage.Model.Facts;

using ValidationRules.Hosting.Common.Settings;

namespace NuClear.ValidationRules.StateInitialization.Host.Kafka.Rulesets
{
    public sealed class RulesetFactsBulkCommandFactory : IBulkCommandFactory<Confluent.Kafka.Message>
    {
        private readonly IDeserializer<Confluent.Kafka.Message, RulesetDto> _deserializer;

        public RulesetFactsBulkCommandFactory(IBusinessModelSettings businessModelSettings)
        {
            _deserializer = new RulesetDtoDeserializer(businessModelSettings);
            AppropriateFlows = new[] { RulesetFactsFlow.Instance };
        }

        public IReadOnlyCollection<IMessageFlow> AppropriateFlows { get; }

        IReadOnlyCollection<ICommand> IBulkCommandFactory<Confluent.Kafka.Message>.CreateCommands(IReadOnlyCollection<Confluent.Kafka.Message> messages)
        {
            var deserializedDtos = messages.AsParallel()
                                           .Select(message => _deserializer.Deserialize(message))
                                           .AsSequential()
                                           .Aggregate(new List<RulesetDto>(messages.Count),
                                                      (dtos, collection) =>
                                                          {
                                                              dtos.AddRange(collection);
                                                              return dtos;
                                                          });
            if (deserializedDtos.Count == 0)
            {
                return Array.Empty<ICommand>();
            }

            return new[]
                {
                    new KafkaReplicationActor.BulkInsertDataObjectsCommand(typeof(Ruleset), deserializedDtos),
                    new KafkaReplicationActor.BulkInsertDataObjectsCommand(typeof(Ruleset.AssociatedRule), deserializedDtos),
                    new KafkaReplicationActor.BulkInsertDataObjectsCommand(typeof(Ruleset.DeniedRule), deserializedDtos),
                    new KafkaReplicationActor.BulkInsertDataObjectsCommand(typeof(Ruleset.QuantitativeRule), deserializedDtos),
                    new KafkaReplicationActor.BulkInsertDataObjectsCommand(typeof(Ruleset.RulesetProject), deserializedDtos)
                };
        }
    }
}
