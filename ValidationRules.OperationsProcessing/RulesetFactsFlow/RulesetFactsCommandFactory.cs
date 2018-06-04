using System;
using System.Collections.Generic;

using NuClear.OperationsProcessing.Transports.Kafka;
using NuClear.Replication.Core;
using NuClear.River.Hosting.Common.Settings;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Dto;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.OperationsProcessing.RulesetFactsFlow
{
    public sealed class RulesetFactsCommandFactory : ICommandFactory<KafkaMessage>
    {
        private readonly IDeserializer<Confluent.Kafka.Message, RulesetDto> _deserializer;

        public RulesetFactsCommandFactory(IEnvironmentSettings environmentSettings)
        {
            _deserializer = new RulesetDtoDeserializer(environmentSettings);
        }

        IReadOnlyCollection<ICommand> ICommandFactory<KafkaMessage>.CreateCommands(KafkaMessage kafkaMessage)
        {
            var deserializedDtos = _deserializer.Deserialize(kafkaMessage.Message);
            if (deserializedDtos.Count == 0)
            {
                return Array.Empty<ICommand>();
            }

            return new[]
                {
                    new ReplaceDataObjectCommand(typeof(Ruleset), deserializedDtos),
                    new ReplaceDataObjectCommand(typeof(Ruleset.AssociatedRule), deserializedDtos),
                    new ReplaceDataObjectCommand(typeof(Ruleset.DeniedRule), deserializedDtos),
                    new ReplaceDataObjectCommand(typeof(Ruleset.QuantitativeRule), deserializedDtos),
                    new ReplaceDataObjectCommand(typeof(Ruleset.RulesetProject), deserializedDtos)
                };
        }
    }
}