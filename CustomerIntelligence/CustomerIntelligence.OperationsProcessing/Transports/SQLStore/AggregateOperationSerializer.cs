using System;
using System.Xml.Linq;

using NuClear.CustomerIntelligence.Domain.EntityTypes;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Identities.Operations;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore
{
    public sealed class XmlOperationSerializer : IOperationSerializer
    {
        public IOperation Deserialize(PerformedOperationFinalProcessing operation)
        {
            var context = XElement.Parse(operation.Context);
            if (operation.OperationId == InitializeAggregateOperationIdentity.Instance.Guid)
            {
                return new InitializeAggregate((int)context.Attribute("entityType"), (long)context.Attribute("entityId"));
            }

            if (operation.OperationId == RecalculateAggregateOperationIdentity.Instance.Guid)
            {
                return new RecalculateAggregate((int)context.Attribute("entityType"), (long)context.Attribute("entityId"));
            }

            if (operation.OperationId == DestroyAggregateOperationIdentity.Instance.Guid)
            {
                return new DestroyAggregate((int)context.Attribute("entityType"), (long)context.Attribute("entityId"));
            }

            if (operation.OperationId == RecalculateAggregatePartOperationIdentity.Instance.Guid)
            {
                return new RecalculateAggregatePart(
                    (int)context.Attribute("aggregateTypeId"),
                    (long)context.Attribute("aggregateInstanceId"),
                    (int)context.Attribute("aggregatePartTypeId"),
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
                                 new XAttribute("entityType", initializeAggregate.EntityTypeId),
                                 new XAttribute("entityId", initializeAggregate.EntityId));
            }

            var recalculateAggregate = operation as RecalculateAggregate;
            if (recalculateAggregate != null)
            {
                return CreatePbo(RecalculateAggregateOperationIdentity.Instance.Guid,
                                 new XAttribute("entityType", recalculateAggregate.EntityTypeId),
                                 new XAttribute("entityId", recalculateAggregate.EntityId));
            }

            var destroyAggregate = operation as DestroyAggregate;
            if (destroyAggregate != null)
            {
                return CreatePbo(DestroyAggregateOperationIdentity.Instance.Guid,
                                 new XAttribute("entityType", destroyAggregate.EntityTypeId),
                                 new XAttribute("entityId", destroyAggregate.EntityId));
            }

            var recalculateStatisticsOperation = operation as RecalculateAggregatePart;
            if (recalculateStatisticsOperation != null)
            {
                return CreatePbo(RecalculateAggregatePartOperationIdentity.Instance.Guid,
                                 new XAttribute("aggregateTypeId", recalculateStatisticsOperation.AggregateTypeId),
                                 new XAttribute("aggregateInstanceId", recalculateStatisticsOperation.AggregateInstanceId),
                                 new XAttribute("aggregatePartTypeId", recalculateStatisticsOperation.EntityTypeId),
                                 new XAttribute("aggregatePartInstanceId", recalculateStatisticsOperation.EntityInstanceId));
            }

            throw new ArgumentException($"unsuppoted command {operation.GetType().Name}", nameof(operation));
        }

        private static IOperation ForProjectCategory(long projectId, long categoryId)
            => new RecalculateAggregatePart(EntityTypeProjectStatistics.Instance.Id, projectId, EntityTypeProjectCategoryStatistics.Instance.Id, categoryId);

        private static IOperation ForProject(long projectId)
            => new RecalculateAggregate(EntityTypeProjectStatistics.Instance.Id, projectId);

        private static PerformedOperationFinalProcessing CreatePbo(Guid operationId, params XAttribute[] attributes)
        {
            return new PerformedOperationFinalProcessing
            {
                OperationId = operationId,
                Context = new XElement("params", attributes).ToString(SaveOptions.DisableFormatting)
            };
        }
    }
}