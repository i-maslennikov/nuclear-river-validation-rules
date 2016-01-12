using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Model.Operations;
using NuClear.Messaging.API.Flows;
using NuClear.Model.Common.Entities;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Identities.Operations;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore
{
    public sealed class AggregateOperationSerializer : IOperationSerializer<AggregateOperation>
    {
        private readonly IEntityTypeMappingRegistry<ISubDomain> _registry;

        private static readonly Dictionary<Guid, Type> OperationIdRegistry =
            new Dictionary<Guid, Type>
            {
                { InitializeAggregateOperationIdentity.Instance.Guid, typeof(InitializeAggregate) },
                { RecalculateAggregateOperationIdentity.Instance.Guid, typeof(RecalculateAggregate) },
                { DestroyAggregateOperationIdentity.Instance.Guid, typeof(DestroyAggregate) }
            };

        private static readonly Dictionary<Type, Guid> OperationTypeRegistry =
            OperationIdRegistry.ToDictionary(x => x.Value, x => x.Key);

        public AggregateOperationSerializer(IEntityTypeMappingRegistry<ISubDomain> registry)
        {
            _registry = registry;
        }

        public AggregateOperation Deserialize(PerformedOperationFinalProcessing operation)
        {
            Type operationType;
            if (!OperationIdRegistry.TryGetValue(operation.OperationId, out operationType))
            {
                throw new ArgumentException($"Unknown operation id {operation.OperationId}", nameof(operation));
            }

            IEntityType entityName;
            if (!_registry.TryParse(operation.EntityTypeId, out entityName))
            {
                throw new ArgumentException($"Unknown entity id {operation.EntityTypeId}", nameof(operation));
            }

            return (AggregateOperation)Activator.CreateInstance(operationType, _registry.GetEntityType(entityName), operation.EntityId);
        }

        public PerformedOperationFinalProcessing Serialize(AggregateOperation operation, IMessageFlow targetFlow)
        {
            var entityType = _registry.GetEntityName(operation.AggregateType);
            return new PerformedOperationFinalProcessing
            {
                CreatedOn = DateTime.UtcNow,
                MessageFlowId = targetFlow.Id,
                EntityId = operation.AggregateId,
                EntityTypeId = entityType.Id,
                OperationId = GetIdentity(operation),
            };
        }

        private static Guid GetIdentity(AggregateOperation operation)
        {
            Guid guid;
            if (OperationTypeRegistry.TryGetValue(operation.GetType(), out guid))
            {
                return guid;
            }

            throw new ArgumentException($"Unknown operation type {operation.GetType().Name}", nameof(operation));
        }
    }
}
