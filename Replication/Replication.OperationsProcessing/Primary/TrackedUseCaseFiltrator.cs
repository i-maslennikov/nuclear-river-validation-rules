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
        private readonly IEntityTypeExplicitMapping _entityTypeExplicitMapping;
        private readonly IOperationRegistry<TSubDomain> _operationsRegistry;

        public TrackedUseCaseFiltrator(
            ITracer tracer,
            IEntityTypeMappingRegistry<TSubDomain> entityTypeRegistry,
            IEntityTypeExplicitMapping entityTypeExplicitMapping,
            IOperationRegistry<TSubDomain> operationsRegistry)
        {
            _tracer = tracer;
            _entityTypeRegistry = entityTypeRegistry;
            _entityTypeExplicitMapping = entityTypeExplicitMapping;
            _operationsRegistry = operationsRegistry;
        }

        public IReadOnlyDictionary<IEntityType, HashSet<long>> Filter(TrackedUseCase trackedUseCase)
        {
            var filteredOperations = FilterByIgnoredOperations(trackedUseCase);
            var changes = FilterByEntityTypes(filteredOperations);

            return changes;
        }

        private IEnumerable<OperationDescriptor> FilterByIgnoredOperations(TrackedUseCase message)
        {
            var operations = (IEnumerable<OperationDescriptor>)message.Operations;

            var ignoredOperations = operations.Where(x => _operationsRegistry.IsIgnoredOperation(x.OperationIdentity));
            var operationIds = ignoredOperations.Aggregate(new HashSet<Guid>(), (hashSet, operation) =>
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
            var dictionary = new Dictionary<IEntityType, HashSet<long>>();

            foreach (var operation in operations)
            {
                foreach (var change in operation.AffectedEntities.Changes)
                {
                    var entityType = _entityTypeExplicitMapping.MapEntityType(change.Key);

                    if (!_entityTypeRegistry.EntityMapping.ContainsKey(entityType))
                    {
                        continue;
                    }

                    if (!_operationsRegistry.IsAllowedOperation(operation.OperationIdentity))
                    {
                        _tracer.WarnFormat("Received well-known entity '{0}' frow unknown ERM operation '{1}'", change.Key, operation.OperationIdentity);
                    }

                    HashSet<long> hashSet;
                    if (!dictionary.TryGetValue(entityType, out hashSet))
                    {
                        hashSet = new HashSet<long>();
                        dictionary.Add(entityType, hashSet);
                    }

                    hashSet.UnionWith(change.Value.Keys);
                }
            }

            return dictionary;
        }
    }
}