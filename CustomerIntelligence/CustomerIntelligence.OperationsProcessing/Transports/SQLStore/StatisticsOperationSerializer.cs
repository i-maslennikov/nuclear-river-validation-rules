using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Messaging.API.Flows;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.OperationsProcessing.Identities.Operations;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore
{
    public sealed class StatisticsOperationSerializer : IOperationSerializer<RecalculateStatisticsOperation>
    {
        private static readonly Dictionary<Guid, Type> OperationIdRegistry =
            new Dictionary<Guid, Type>
            {
                { StatisticsOperationIdentity.Instance.Guid, typeof(RecalculateStatisticsOperation) }
            };

        private static readonly Dictionary<Type, Guid> OperationTypeRegistry =
            OperationIdRegistry.ToDictionary(x => x.Value, x => x.Key);

        public RecalculateStatisticsOperation Deserialize(PerformedOperationFinalProcessing operation)
        {
            var context = XElement.Parse(operation.Context);

            return new RecalculateStatisticsOperation(new StatisticsKey
                {
                    ProjectId = (long)context.Attribute("Project"),
                    CategoryId = (long?)context.Attribute("Category"),
                });
        }

        public PerformedOperationFinalProcessing Serialize(RecalculateStatisticsOperation operation, IMessageFlow targetFlow)
        {
            return new PerformedOperationFinalProcessing
            {
                CreatedOn = DateTime.UtcNow,
                MessageFlowId = targetFlow.Id,
                Context = Serialize(operation).ToString(),
                OperationId = GetIdentity(operation),
            };
        }

        private static XElement Serialize(RecalculateStatisticsOperation operation)
        {
            return new XElement("RecalulateStatistics",
                                new XAttribute("Project", operation.EntityId.ProjectId),
                                operation.EntityId.CategoryId.HasValue ? new XAttribute("Category", operation.EntityId.CategoryId) : null);
        }

        private static Guid GetIdentity(RecalculateStatisticsOperation operation)
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