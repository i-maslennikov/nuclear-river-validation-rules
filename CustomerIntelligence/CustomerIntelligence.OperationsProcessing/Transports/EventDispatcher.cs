using System.Collections.Generic;

using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.Messaging.API.Flows;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Transports
{
    public sealed class EventDispatcher : IEventDispatcher
    {
        public IDictionary<IMessageFlow, IReadOnlyCollection<IEvent>> Dispatch(IReadOnlyCollection<IEvent> events)
        {
            var aggregateFlow = new List<IEvent>();
            var statisticsFlow = new List<IEvent>();

            foreach (var @event in events)
            {
                if (@event is IDataObjectEvent<StatisticsKey>)
                {
                    statisticsFlow.Add(@event);
                }
                else
                {
                    aggregateFlow.Add(@event);
                }
            }

            return new Dictionary<IMessageFlow, IReadOnlyCollection<IEvent>>
                {
                    { StatisticsFlow.Instance, statisticsFlow },
                    { AggregatesFlow.Instance, aggregateFlow },
                };
        }
    }
}
