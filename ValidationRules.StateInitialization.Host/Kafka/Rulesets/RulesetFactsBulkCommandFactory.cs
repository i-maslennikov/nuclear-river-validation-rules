using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.SqlServer.Management.Smo;

using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core;
using NuClear.StateInitialization.Core;
using NuClear.ValidationRules.OperationsProcessing;
using NuClear.ValidationRules.OperationsProcessing.Facts.RulesetFactsFlow;
using NuClear.ValidationRules.Replication.Dto;

using ValidationRules.Hosting.Common.Settings;

using Schema = NuClear.ValidationRules.Storage.Schema;

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

            var bulkReplicateCommands =
                DataObjectTypesProviderFactory.RulesetFactTypes
                                              .Select(factType => new KafkaReplicationActor.BulkInsertDataObjectsCommand(factType, deserializedDtos))
                                              .OfType<ICommand>();

            var updateStatisticsCommands =
                DataObjectTypesProviderFactory.RulesetFactTypes
                                              .Select(factType => Schema.Facts.GetTableName(factType))
                                              .Select(table => new UpdateTableStatisticsActor.UpdateTableStatisticsCommand(table,
                                                                                                                           StatisticsTarget.All,
                                                                                                                           StatisticsScanType.FullScan));
            return bulkReplicateCommands.Concat(updateStatisticsCommands)
                                        .ToList();
        }
    }
}
