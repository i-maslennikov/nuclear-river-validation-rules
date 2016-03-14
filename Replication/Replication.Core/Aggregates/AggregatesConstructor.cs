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
    public sealed class AggregatesConstructor<TSubDomain> : IAggregatesConstructor
        where TSubDomain : ISubDomain
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IAggregateProcessorFactory _aggregateProcessorFactory;
        private readonly IEntityTypeMappingRegistry<TSubDomain> _entityTypeMappingRegistry;

        public AggregatesConstructor(IMetadataProvider metadataProvider, IAggregateProcessorFactory aggregateProcessorFactory, IEntityTypeMappingRegistry<TSubDomain> entityTypeMappingRegistry)
        {
            _metadataProvider = metadataProvider;
            _aggregateProcessorFactory = aggregateProcessorFactory;
            _entityTypeMappingRegistry = entityTypeMappingRegistry;
        }

        public void Execute(IReadOnlyCollection<IOperation> commands)
        {
            using (Probe.Create("ETL2 Transforming"))
            {
                var commandsByType = commands.GroupBy(x => x.GetType()).OrderBy(x => x.Key, new AggregateOperationPriorityComparer());

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                                                              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
                {
                    foreach (var group in commandsByType)
                    {
                        if (group.Key == typeof(InitializeAggregate))
                        {
                            Execute(group.Cast<InitializeAggregate>());
                        }
                        else if (group.Key == typeof(RecalculateAggregate))
                        {
                            Execute(group.Cast<RecalculateAggregate>());
                        }
                        else if (group.Key == typeof(DestroyAggregate))
                        {
                            Execute(group.Cast<DestroyAggregate>());
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

        private void Execute(IEnumerable<InitializeAggregate> enumerable)
        {
            foreach (var slice in enumerable.GroupBy(c => c.EntityTypeId))
            {
                var type = ParseEntityType(slice.Key);
                var processor = CreateProcessor(type);
                using (Probe.Create($"ETL2 Initialize {type.Name}"))
                {
                    processor.Initialize(slice.ToArray());
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

        private void Execute(IEnumerable<DestroyAggregate> enumerable)
        {
            foreach (var slice in enumerable.GroupBy(c => c.EntityTypeId))
            {
                var type = ParseEntityType(slice.Key);
                var processor = CreateProcessor(type);
                using (Probe.Create($"ETL2 Destroy {type.Name}"))
                {
                    processor.Destroy(slice.ToArray());
                }
            }
        }

        private IAggregateProcessor CreateProcessor(Type aggregate)
        {
            IMetadataElement aggregateMetadata;
            var metadataId = ReplicationMetadataIdentity.Instance.Id.WithRelative(new Uri($"Aggregates/{aggregate.Name}", UriKind.Relative));
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

