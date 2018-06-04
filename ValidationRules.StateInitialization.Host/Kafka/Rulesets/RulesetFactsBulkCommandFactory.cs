using System;
using System.Collections.Generic;

using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core;
using NuClear.River.Hosting.Common.Settings;
using NuClear.ValidationRules.OperationsProcessing;
using NuClear.ValidationRules.OperationsProcessing.RulesetFactsFlow;
using NuClear.ValidationRules.Replication.Dto;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.StateInitialization.Host.Kafka.Rulesets
{
    public sealed class RulesetFactsBulkCommandFactory : IBulkCommandFactory<Confluent.Kafka.Message>
    {
        private readonly IDeserializer<Confluent.Kafka.Message, RulesetDto> _deserializer;

        public RulesetFactsBulkCommandFactory(IEnvironmentSettings environmentSettings)
        {
            _deserializer = new RulesetDtoDeserializer(environmentSettings);
            AppropriateFlows = new[] { RulesetFactsFlow.Instance };
        }

        public IReadOnlyCollection<IMessageFlow> AppropriateFlows { get; }

        IReadOnlyCollection<ICommand> IBulkCommandFactory<Confluent.Kafka.Message>.CreateCommands(Confluent.Kafka.Message message)
        {
            var deserializedDtos = _deserializer.Deserialize(message);
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
