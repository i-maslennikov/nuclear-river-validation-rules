using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider;
using NuClear.Model.Common.Entities;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Context;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class AggregatesConstructor<TSubDomain> : IAggregatesConstructor
        where TSubDomain : ISubDomain
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IAggregateProcessorFactory _aggregateProcessorFactory;
        private readonly IEntityTypeMappingRegistry<TSubDomain> _mappingRegistry;
        private readonly ISlicer<AggregateProcessorSlice> _slicer;
        private readonly IMetadataUriProvider _metadataUriProvider;

        public AggregatesConstructor(IMetadataProvider metadataProvider, IAggregateProcessorFactory aggregateProcessorFactory, IEntityTypeMappingRegistry<TSubDomain> mappingRegistry, IMetadataUriProvider metadataUriProvider)
        {
            _metadataProvider = metadataProvider;
            _aggregateProcessorFactory = aggregateProcessorFactory;
            _mappingRegistry = mappingRegistry;
            _metadataUriProvider = metadataUriProvider;
            _slicer = new AggregateRecalculationSlicer();
        }

        public void Construct(IEnumerable<AggregateOperation> operations)
        {
            using (Probe.Create("ETL2 Transforming"))
            {
                var operationTypeGroups = operations.GroupBy(x => x.GetType(), x => x.Context)
                                                    .OrderByDescending(x => x.Key, new AggregateOperationPriorityComparer());

                foreach (var operationTypeGroup in operationTypeGroups)
                {
                    Construct(operationTypeGroup.Key, operationTypeGroup);
                }
            }
        }

        private void Construct(Type operation, IEnumerable<Predicate> operations)
                    {
            foreach (var slice in _slicer.Slice(operations))
            {
                var aggregateType = ParseAggregateType(slice.AggregateTypeId);
                var processor = CreateProcessor(aggregateType);

                    using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                                                                  new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
                    {
                        using (Probe.Create("ETL2 Transforming", aggregateType.Name))
                        {

                            if (operation == typeof(InitializeAggregate))
                            {
                            processor.Initialize(slice);
                            }
                            else if (operation == typeof(RecalculateAggregate))
                            {
                            processor.Recalculate(slice);
                            }
                            else if (operation == typeof(DestroyAggregate))
                            {
                            processor.Destroy(slice);
                            }
                            else
                            {
                                throw new InvalidOperationException($"The command of type {operation.Name} is not supported");
                            }
                        }

                        transaction.Complete();
                    }
            }
        }

        private Type ParseAggregateType(int entityTypeId)
        {
            IEntityType aggregateEntityType;
            Type aggregateType;
            if (!_mappingRegistry.TryParse(entityTypeId, out aggregateEntityType) ||
                !_mappingRegistry.TryGetEntityType(aggregateEntityType, out aggregateType))
            {
                throw new NotSupportedException($"Unknown aggregate '{entityTypeId}'");
            }

            return aggregateType;
        }

        private IAggregateProcessor CreateProcessor(Type aggregateType)
        {
            IMetadataElement aggregateMetadata;
            var metadataId = _metadataUriProvider.GetFor(aggregateType);
            if (!_metadataProvider.TryGetMetadata(metadataId, out aggregateMetadata))
            {
                throw new NotSupportedException($"The aggregate of type '{aggregateType.Name}' is not supported.");
            }

            return _aggregateProcessorFactory.Create(aggregateMetadata);
                }

        public sealed class AggregateRecalculationSlicer : ISlicer<AggregateProcessorSlice>
        {
            public IEnumerable<AggregateProcessorSlice> Slice(IEnumerable<Predicate> predicates)
            {
                return predicates.GroupBy(p => PredicateProperty.EntityType.GetValue(p), p => PredicateProperty.EntityId.GetValue(p))
                                 .Select(group => new AggregateProcessorSlice { AggregateTypeId = group.Key, AggregateIds = group.ToArray() });
            }
        }
    }
}

