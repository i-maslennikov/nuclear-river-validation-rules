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

        public void Execute(IEnumerable<AggregateOperation> commands)
        {
            using (Probe.Create("ETL2 Transforming"))
            {
                var slices = commands.GroupBy(x => new { Operation = x.GetType(), x.EntityTypeId })
                                     .OrderByDescending(x => x.Key.Operation, new AggregateOperationPriorityComparer());

                foreach (var slice in slices)
                {
                    Execute(slice.Key.Operation, ParseEntityType(slice.Key.EntityTypeId), slice);
                }
            }
        }

        private void Execute(Type command, Type aggregate, IEnumerable<AggregateOperation> commands)
        {
            var processor = CreateProcessor(aggregate);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                                                          new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
            {
                using (Probe.Create($"ETL2 {command.Name} {aggregate.Name}"))
                {
                    if (command == typeof(InitializeAggregate))
                    {
                        processor.Initialize(commands.Cast<InitializeAggregate>().ToArray());
                    }
                    else if (command == typeof(RecalculateAggregate))
                    {
                        processor.Recalculate(commands.Cast<RecalculateAggregate>().ToArray());
                    }
                    else if (command == typeof(DestroyAggregate))
                    {
                        processor.Destroy(commands.Cast<DestroyAggregate>().ToArray());
                    }
                    else
                    {
                        throw new InvalidOperationException($"The command of type {command.Name} is not supported");
                    }
                }

                transaction.Complete();
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

