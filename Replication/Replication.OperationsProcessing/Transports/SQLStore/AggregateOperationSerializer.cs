using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Messaging.API.Flows;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Identities.Operations;
using NuClear.River.Common.Metadata.Context;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public sealed class AggregateOperationSerializer : IOperationSerializer<AggregateOperation>
    {
        private readonly IPredicateXmlSerializer _predicateXmlSerializer;

        private static readonly Dictionary<Guid, Type> OperationIdRegistry =
            new Dictionary<Guid, Type>
            {
                { InitializeAggregateOperationIdentity.Instance.Guid, typeof(InitializeAggregate) },
                { RecalculateAggregateOperationIdentity.Instance.Guid, typeof(RecalculateAggregate) },
                { DestroyAggregateOperationIdentity.Instance.Guid, typeof(DestroyAggregate) },
                { StatisticsOperationIdentity.Instance.Guid, typeof(RecalculateStatisticsOperation) }
            };

        private static readonly Dictionary<Type, Guid> OperationTypeRegistry =
            OperationIdRegistry.ToDictionary(x => x.Value, x => x.Key);

        public AggregateOperationSerializer(IPredicateXmlSerializer predicateXmlSerializer)
        {
            _predicateXmlSerializer = predicateXmlSerializer;
        }

        public AggregateOperation Deserialize(PerformedOperationFinalProcessing operation)
        {
            Type operationType;
            if (!OperationIdRegistry.TryGetValue(operation.OperationId, out operationType))
            {
                throw new ArgumentException($"Unknown operation id {operation.OperationId}", nameof(operation));
            }

            return (AggregateOperation)Activator.CreateInstance(operationType, _predicateXmlSerializer.Deserialize(XElement.Parse(operation.Context)));
        }

        public PerformedOperationFinalProcessing Serialize(AggregateOperation operation, IMessageFlow targetFlow)
        {
            return new PerformedOperationFinalProcessing
            {
                CreatedOn = DateTime.UtcNow,
                MessageFlowId = targetFlow.Id,
                Context = _predicateXmlSerializer.Serialize(operation.Context).ToString(SaveOptions.DisableFormatting),
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
