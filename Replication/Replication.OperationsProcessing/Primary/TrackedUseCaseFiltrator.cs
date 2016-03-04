using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common.Entities;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Tracing.API;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class TrackedUseCaseFiltrator<TSubDomain>
        where TSubDomain : ISubDomain
    {
        private readonly ITracer _tracer;
        private readonly IEntityTypeMappingRegistry<TSubDomain> _entityTypeRegistry;
        private readonly OperationRegistry<TSubDomain> _operationsRegistry;

        public TrackedUseCaseFiltrator(ITracer tracer, IEntityTypeMappingRegistry<TSubDomain> entityTypeRegistry, OperationRegistry<TSubDomain> operationsRegistry)
        {
            _tracer = tracer;
            _entityTypeRegistry = entityTypeRegistry;
            _operationsRegistry = operationsRegistry;
        }

        public IReadOnlyDictionary<IEntityType, HashSet<long>> Filter(TrackedUseCase trackedUseCase)
        {
            var filteredOperations = FilterByDisallowedOperations(trackedUseCase);
            var changes = FilterByEntityTypes(filteredOperations);

            return changes;
        }

        private IEnumerable<OperationDescriptor> FilterByDisallowedOperations(TrackedUseCase message)
        {
            var operations = (IEnumerable<OperationDescriptor>)message.Operations;

            var disallowedOperations = operations.Where(x => _operationsRegistry.IsDisallowedOperation(x.OperationIdentity));
            var operationIds = disallowedOperations.Aggregate(new HashSet<Guid>(), (hashSet, operation) =>
            {
                hashSet.Add(operation.Id);
                hashSet.UnionWith(message.GetNestedOperations(operation.Id).Select(x => x.Id));
                return hashSet;
            });

            if (operationIds.Any())
            {
                operations = operations.Where(x => !operationIds.Contains(x.Id));
            }

            return operations;
        }

        private IReadOnlyDictionary<IEntityType, HashSet<long>> FilterByEntityTypes(IEnumerable<OperationDescriptor> operations)
        {
            var changes = operations.Aggregate(new Dictionary<IEntityType, HashSet<long>>(), (x, operation) =>
            {
                x = operation.AffectedEntities.Changes.Aggregate(x, (y, change) =>
                {
                    IEntityType entityType;
                    if (!_operationsRegistry.TryGetExplicitlyMappedEntityType(change.Key, out entityType))
                    {
                        entityType = change.Key;
                    }

                    if (!_entityTypeRegistry.EntityMapping.ContainsKey(entityType))
                    {
                        return y;
                    }

                    if (!_operationsRegistry.IsAllowedOperation(operation.OperationIdentity))
                    {
                        _tracer.WarnFormat("Received well-known entity '{0}' frow unknown ERM operation '{1}'", change.Key, operation.OperationIdentity);
                    }

                    HashSet<long> hashSet;
                    if (!y.TryGetValue(entityType, out hashSet))
                    {
                        hashSet = new HashSet<long>();
                        y.Add(entityType, hashSet);
                    }

                    hashSet.UnionWith(change.Value.Keys);

                    return y;
                });

                return x;
            });

            return changes;
        }
    }
}