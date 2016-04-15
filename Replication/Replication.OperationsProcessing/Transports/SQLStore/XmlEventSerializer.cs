using System;
using System.Xml.Linq;

using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Identities.Operations;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public sealed class XmlEventSerializer : IEventSerializer
    {
        private readonly IEntityReferenceSerializer _entityReferenceSerializer;

        public XmlEventSerializer(IEntityReferenceSerializer entityReferenceSerializer)
        {
            _entityReferenceSerializer = entityReferenceSerializer;
        }

        public IEvent Deserialize(PerformedOperationFinalProcessing message)
        {
            var context = XElement.Parse(message.Context);
            if (message.OperationId == InitializeAggregateOperationIdentity.Instance.Guid)
            {
                return new InitializeAggregate(_entityReferenceSerializer.Deserialize(context.Element("root")));
            }

            if (message.OperationId == RecalculateAggregateOperationIdentity.Instance.Guid)
            {
                return new RecalculateAggregate(_entityReferenceSerializer.Deserialize(context.Element("root")));
            }

            if (message.OperationId == DestroyAggregateOperationIdentity.Instance.Guid)
            {
                return new DestroyAggregate(_entityReferenceSerializer.Deserialize(context.Element("root")));
            }

            if (message.OperationId == RecalculateAggregatePartOperationIdentity.Instance.Guid)
            {
                return new RecalculateAggregatePart(
                    _entityReferenceSerializer.Deserialize(context.Element("root")),
                    _entityReferenceSerializer.Deserialize(context.Element("part")));
            }

            throw new ArgumentException($"Unknown operation id {message.OperationId}", nameof(message));
        }

        public PerformedOperationFinalProcessing Serialize(IEvent @event)
        {
            var initializeAggregate = @event as InitializeAggregate;
            if (initializeAggregate != null)
            {
                return CreatePbo(InitializeAggregateOperationIdentity.Instance.Guid,
                                 _entityReferenceSerializer.Serialize("root", initializeAggregate.AggregateRoot));
            }

            var recalculateAggregate = @event as RecalculateAggregate;
            if (recalculateAggregate != null)
            {
                return CreatePbo(RecalculateAggregateOperationIdentity.Instance.Guid,
                                 _entityReferenceSerializer.Serialize("root", recalculateAggregate.AggregateRoot));
            }

            var destroyAggregate = @event as DestroyAggregate;
            if (destroyAggregate != null)
            {
                return CreatePbo(DestroyAggregateOperationIdentity.Instance.Guid,
                                 _entityReferenceSerializer.Serialize("root", destroyAggregate.AggregateRoot));
            }

            var recalculateStatisticsOperation = @event as RecalculateAggregatePart;
            if (recalculateStatisticsOperation != null)
            {
                return CreatePbo(RecalculateAggregatePartOperationIdentity.Instance.Guid,
                                 _entityReferenceSerializer.Serialize("root", recalculateStatisticsOperation.AggregateRoot),
                                 _entityReferenceSerializer.Serialize("part", recalculateStatisticsOperation.Entity));
            }

            throw new ArgumentException($"unsuppoted command {@event.GetType().Name}", nameof(@event));
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