using System;
using System.Xml.Linq;

using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Identities.Operations;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public sealed class XmlOperationSerializer : IOperationSerializer
    {
        private readonly IEntityReferenceSerializer _entityReferenceSerializer;

        public XmlOperationSerializer(IEntityReferenceSerializer entityReferenceSerializer)
        {
            _entityReferenceSerializer = entityReferenceSerializer;
        }

        public IOperation Deserialize(PerformedOperationFinalProcessing operation)
        {
            var context = XElement.Parse(operation.Context);
            if (operation.OperationId == InitializeAggregateOperationIdentity.Instance.Guid)
            {
                return new InitializeAggregate(_entityReferenceSerializer.Deserialize(context.Element("root")));
            }

            if (operation.OperationId == RecalculateAggregateOperationIdentity.Instance.Guid)
            {
                return new RecalculateAggregate(_entityReferenceSerializer.Deserialize(context.Element("root")));
            }

            if (operation.OperationId == DestroyAggregateOperationIdentity.Instance.Guid)
            {
                return new DestroyAggregate(_entityReferenceSerializer.Deserialize(context.Element("root")));
            }

            if (operation.OperationId == RecalculateAggregatePartOperationIdentity.Instance.Guid)
            {
                return new RecalculateAggregatePart(
                    _entityReferenceSerializer.Deserialize(context.Element("root")),
                    _entityReferenceSerializer.Deserialize(context.Element("part")));
            }

            throw new ArgumentException($"Unknown operation id {operation.OperationId}", nameof(operation));
        }

        public PerformedOperationFinalProcessing Serialize(IOperation operation)
        {
            var initializeAggregate = operation as InitializeAggregate;
            if (initializeAggregate != null)
            {
                return CreatePbo(InitializeAggregateOperationIdentity.Instance.Guid,
                                 _entityReferenceSerializer.Serialize("root", initializeAggregate.AggregateRoot));
            }

            var recalculateAggregate = operation as RecalculateAggregate;
            if (recalculateAggregate != null)
            {
                return CreatePbo(RecalculateAggregateOperationIdentity.Instance.Guid,
                                 _entityReferenceSerializer.Serialize("root", recalculateAggregate.AggregateRoot));
            }

            var destroyAggregate = operation as DestroyAggregate;
            if (destroyAggregate != null)
            {
                return CreatePbo(DestroyAggregateOperationIdentity.Instance.Guid,
                                 _entityReferenceSerializer.Serialize("root", destroyAggregate.AggregateRoot));
            }

            var recalculateStatisticsOperation = operation as RecalculateAggregatePart;
            if (recalculateStatisticsOperation != null)
            {
                return CreatePbo(RecalculateAggregatePartOperationIdentity.Instance.Guid,
                                 _entityReferenceSerializer.Serialize("root", recalculateStatisticsOperation.AggregateRoot),
                                 _entityReferenceSerializer.Serialize("part", recalculateStatisticsOperation.Entity));
            }

            throw new ArgumentException($"unsuppoted command {operation.GetType().Name}", nameof(operation));
        }

        private static PerformedOperationFinalProcessing CreatePbo(Guid operationId, params object[] content)
        {
            return new PerformedOperationFinalProcessing
            {
                OperationId = operationId,
                Context = new XElement("params", content).ToString(SaveOptions.DisableFormatting)
            };
        }

    }
}