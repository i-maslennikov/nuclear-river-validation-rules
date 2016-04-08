using System.Collections.Generic;

using NuClear.CustomerIntelligence.Domain.EntityTypes;
using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.Messaging.API.Flows;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Transports
{
    public sealed class OperationDispatcher : IOperationDispatcher
    {
        public IDictionary<IMessageFlow, IReadOnlyCollection<IOperation>> Dispatch(IEnumerable<IOperation> operations)
        {
            var aggregateFlow = new List<IOperation>();
            var statisticsFlow = new List<IOperation>();

            foreach (var operation in operations)
            {
                var recalculatePart = operation as RecalculateAggregatePart;
                if (recalculatePart != null && recalculatePart.AggregateRoot.EntityType.Id == EntityTypeProjectStatistics.Instance.Id)
                {
                    statisticsFlow.Add(operation);
                    continue;
                }

                var recalculate = operation as RecalculateAggregate;
                if (recalculate != null && recalculate.AggregateRoot.EntityType.Id == EntityTypeProjectStatistics.Instance.Id)
                {
                    statisticsFlow.Add(operation);
                    continue;
                }

                aggregateFlow.Add(operation);
            }

            return new Dictionary<IMessageFlow, IReadOnlyCollection<IOperation>>
                {
                    { StatisticsFlow.Instance, statisticsFlow },
                    { AggregatesFlow.Instance, aggregateFlow },
                };
        }
    }
}
