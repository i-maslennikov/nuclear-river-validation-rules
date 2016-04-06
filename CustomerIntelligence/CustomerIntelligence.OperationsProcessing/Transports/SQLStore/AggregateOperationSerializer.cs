using System;
using System.Xml.Linq;

using NuClear.Model.Common.Entities;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Identities.Operations;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore
{
    public sealed class XmlOperationSerializer<TSubDomain> : IOperationSerializer
        where TSubDomain : ISubDomain
    {
        private readonly IEntityTypeMappingRegistry<TSubDomain> _typeMappingRegistry;

        public XmlOperationSerializer(IEntityTypeMappingRegistry<TSubDomain> typeMappingRegistry)
        {
            _typeMappingRegistry = typeMappingRegistry;
        }

        public IOperation Deserialize(PerformedOperationFinalProcessing operation)
        {
            var context = XElement.Parse(operation.Context);
            if (operation.OperationId == InitializeAggregateOperationIdentity.Instance.Guid)
            {
                var entityType = ParseEntityType((int)context.Attribute("entityType"));
                return new InitializeAggregate(entityType, (long)context.Attribute("entityId"));
            }

            if (operation.OperationId == RecalculateAggregateOperationIdentity.Instance.Guid)
            {
                var entityType = ParseEntityType((int)context.Attribute("entityType"));
                return new RecalculateAggregate(entityType, (long)context.Attribute("entityId"));
            }

            if (operation.OperationId == DestroyAggregateOperationIdentity.Instance.Guid)
            {
                var entityType = ParseEntityType((int)context.Attribute("entityType"));
                return new DestroyAggregate(entityType, (long)context.Attribute("entityId"));
            }

            if (operation.OperationId == RecalculateAggregatePartOperationIdentity.Instance.Guid)
            {
                var aggregateType = ParseEntityType((int)context.Attribute("aggregateTypeId"));
                var aggregatePartType = ParseEntityType((int)context.Attribute("aggregatePartTypeId"));
                return new RecalculateAggregatePart(
                    aggregateType,
                    (long)context.Attribute("aggregateInstanceId"),
                    aggregatePartType,
                    (long)context.Attribute("aggregatePartInstanceId"));
            }

            throw new ArgumentException($"Unknown operation id {operation.OperationId}", nameof(operation));
        }

        public PerformedOperationFinalProcessing Serialize(IOperation operation)
        {
            var initializeAggregate = operation as InitializeAggregate;
            if (initializeAggregate != null)
            {
                return CreatePbo(InitializeAggregateOperationIdentity.Instance.Guid,
                                 new XAttribute("entityType", initializeAggregate.EntityType.Id),
                                 new XAttribute("entityId", initializeAggregate.EntityId));
            }

            var recalculateAggregate = operation as RecalculateAggregate;
            if (recalculateAggregate != null)
            {
                return CreatePbo(RecalculateAggregateOperationIdentity.Instance.Guid,
                                 new XAttribute("entityType", recalculateAggregate.EntityType.Id),
                                 new XAttribute("entityId", recalculateAggregate.EntityId));
            }

            var destroyAggregate = operation as DestroyAggregate;
            if (destroyAggregate != null)
            {
                return CreatePbo(DestroyAggregateOperationIdentity.Instance.Guid,
                                 new XAttribute("entityType", destroyAggregate.EntityType.Id),
                                 new XAttribute("entityId", destroyAggregate.EntityId));
            }

            var recalculateStatisticsOperation = operation as RecalculateAggregatePart;
            if (recalculateStatisticsOperation != null)
            {
                return CreatePbo(RecalculateAggregatePartOperationIdentity.Instance.Guid,
                                 new XAttribute("aggregateTypeId", recalculateStatisticsOperation.AggregateType.Id),
                                 new XAttribute("aggregateInstanceId", recalculateStatisticsOperation.AggregateInstanceId),
                                 new XAttribute("aggregatePartTypeId", recalculateStatisticsOperation.EntityType.Id),
                                 new XAttribute("aggregatePartInstanceId", recalculateStatisticsOperation.EntityInstanceId));
            }

            throw new ArgumentException($"unsuppoted command {operation.GetType().Name}", nameof(operation));
        }

        private static PerformedOperationFinalProcessing CreatePbo(Guid operationId, params XAttribute[] attributes)
        {
            return new PerformedOperationFinalProcessing
            {
                OperationId = operationId,
                Context = new XElement("params", attributes).ToString(SaveOptions.DisableFormatting)
            };
        }

        private IEntityType ParseEntityType(int id)
        {
            IEntityType result;
            return _typeMappingRegistry.TryParse(id, out result)
                       ? result
                       : new UnknownEntityType(id);
        }
    }
}