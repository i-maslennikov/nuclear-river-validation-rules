using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Model.Common.Entities;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Identities;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.Core.Aggregates
{
    public class StatisticsRecalculator<TSubDomain> : IStatisticsRecalculator
        where TSubDomain : ISubDomain
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IAggregateProcessorFactory _aggregateProcessorFactory;
        private readonly IEntityTypeMappingRegistry<TSubDomain> _entityTypeMappingRegistry;

        public StatisticsRecalculator(IMetadataProvider metadataProvider, IAggregateProcessorFactory aggregateProcessorFactory, IEntityTypeMappingRegistry<TSubDomain> entityTypeMappingRegistry)
        {
            _metadataProvider = metadataProvider;
            _aggregateProcessorFactory = aggregateProcessorFactory;
            _entityTypeMappingRegistry = entityTypeMappingRegistry;
        }

        public void Execute(IReadOnlyCollection<IOperation> commands)
        {
            using (Probe.Create("Recalculate Statistics Operations"))
            {
                var commandsByType = commands.GroupBy(x => x.GetType()).OrderBy(x => x.Key);

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                                                              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
                {
                    foreach (var group in commandsByType)
                    {
                        if (group.Key == typeof(RecalculateAggregate))
                        {
                            Execute(group.Cast<RecalculateAggregate>());
                        }
                        if (group.Key == typeof(RecalculateAggregatePart))
                        {
                            Execute(group.Cast<RecalculateAggregatePart>());
                        }
                        else
                        {
                            throw new InvalidOperationException($"The command of type {group.Key.Name} is not supported");
                        }
                    }

                    transaction.Complete();
                }
            }
        }

        private void Execute(IEnumerable<RecalculateAggregate> enumerable)
        {
            foreach (var slice in enumerable.GroupBy(c => c.EntityTypeId))
            {
                var type = ParseEntityType(slice.Key);
                var processor = CreateProcessor(type);
                using (Probe.Create($"ETL2 Recalculate {type.Name}"))
                {
                    processor.Recalculate(slice.ToArray());
                }
            }
        }

        private void Execute(IEnumerable<RecalculateAggregatePart> enumerable)
        {
            foreach (var slice in enumerable.GroupBy(c => new { c.AggregateTypeId, c.EntityTypeId }))
            {
                var aggregateType = ParseEntityType(slice.Key.AggregateTypeId);
                var entityType = ParseEntityType(slice.Key.EntityTypeId);
                var processor = CreateProcessor(aggregateType);
                using (Probe.Create($"ETL2 Recalculate {aggregateType.Name} Part {entityType.Name}"))
                {
                    processor.RecalculatePartially(slice.ToArray(), entityType);
                }
            }
        }

        private IAggregateProcessor CreateProcessor(Type aggregate)
        {
            IMetadataElement aggregateMetadata;
            var metadataId = ReplicationMetadataIdentity.Instance.Id.WithRelative(new Uri($"Statistics/{aggregate.Name}", UriKind.Relative));
            if (!_metadataProvider.TryGetMetadata(metadataId, out aggregateMetadata))
            {
                throw new NotSupportedException($"The aggregate of type '{aggregate.Name}' is not supported.");
            }

            var processor = _aggregateProcessorFactory.Create(aggregateMetadata);
            return processor;
        }

        private Type ParseEntityType(int entityTypeId)
        {
            Type type;
            IEntityType entityType;
            if (_entityTypeMappingRegistry.TryParse(entityTypeId, out entityType) &&
                _entityTypeMappingRegistry.TryGetEntityType(entityType, out type))
            {
                return type;
            }

            throw new ArgumentException($"unknown entity type id {entityTypeId}");
        }
    }
}
