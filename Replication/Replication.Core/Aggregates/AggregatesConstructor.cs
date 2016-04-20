using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider;
using NuClear.Model.Common.Entities;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Elements;
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
        private readonly IMetadataUriProvider _uriProvider;

        public AggregatesConstructor(IMetadataProvider metadataProvider, IAggregateProcessorFactory aggregateProcessorFactory, IEntityTypeMappingRegistry<TSubDomain> entityTypeMappingRegistry, IMetadataUriProvider uriProvider)
        {
            _metadataProvider = metadataProvider;
            _aggregateProcessorFactory = aggregateProcessorFactory;
            _entityTypeMappingRegistry = entityTypeMappingRegistry;
            _uriProvider = uriProvider;
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
                        else if(group.Key == typeof(RecalculateAggregatePart))
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

        private void Execute(IEnumerable<InitializeAggregate> enumerable)
        {
            foreach (var slice in enumerable.GroupBy(c => c.AggregateRoot.EntityType))
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
            foreach (var slice in enumerable.GroupBy(c => c.AggregateRoot.EntityType))
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
            foreach (var slice in enumerable.GroupBy(c => c.AggregateRoot.EntityType))
            {
                var type = ParseEntityType(slice.Key);
                var processor = CreateProcessor(type);
                using (Probe.Create($"ETL2 Destroy {type.Name}"))
                {
                    processor.Destroy(slice.ToArray());
                }
            }
        }

        private void Execute(IEnumerable<RecalculateAggregatePart> enumerable)
        {
            foreach (var slice in enumerable.GroupBy(c => new { AggregateRootType = c.AggregateRoot.EntityType, c.Entity.EntityType }))
            {
                var aggregateType = ParseEntityType(slice.Key.AggregateRootType);
                var entityType = ParseEntityType(slice.Key.EntityType);
                var processor = CreateProcessor(aggregateType);
                using (Probe.Create($"ETL2 Recalculate {aggregateType.Name} Part {entityType.Name}"))
                {
                    processor.Recalculate(entityType, slice.ToArray());
                }
            }
        }

        // todo: если перевести метаданные на идентификаоры, основанные не на тменах типов, а идентификаторах - то от этого парсинга можно избавиться
        private IAggregateProcessor CreateProcessor(Type aggregate)
        {
            IMetadataElement aggregateMetadata;
            var metadataId = _uriProvider.GetFor(aggregate);
            if (!_metadataProvider.TryGetMetadata(metadataId, out aggregateMetadata))
            {
                throw new NotSupportedException($"The aggregate of type '{aggregate.Name}' is not supported.");
            }

            var processor = _aggregateProcessorFactory.Create(aggregateMetadata);
            return processor;
        }

        private Type ParseEntityType(IEntityType entityType)
        {
            Type type;
            if (_entityTypeMappingRegistry.TryGetEntityType(entityType, out type))
            {
                return type;
            }

            throw new ArgumentException($"unknown entity type id {entityType.Id}");
        }
    }
}

