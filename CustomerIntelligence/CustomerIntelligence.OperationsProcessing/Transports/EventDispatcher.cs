using System.Collections.Generic;

using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.CustomerIntelligence.Replication;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing.Transports;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Transports
{
    public sealed class EventDispatcher : IEventDispatcher
    {
        public IDictionary<IMessageFlow, IReadOnlyCollection<IEvent>> Dispatch(IReadOnlyCollection<IEvent> events)
        {
            var commonEvents = new List<IEvent>();
            var statisticsEvents = new List<IEvent>();

            foreach (var @event in events)
            {
                if (@event is DataObjectReplacedEvent || @event is RelatedDataObjectOutdatedEvent<StatisticsKey>)
                {
                    statisticsEvents.Add(@event);
                }
                else
                {
                    commonEvents.Add(@event);
                }
            }

            return new Dictionary<IMessageFlow, IReadOnlyCollection<IEvent>>
                {
                    { StatisticsEventsFlow.Instance, statisticsEvents },
                    { CommonEventsFlow.Instance, commonEvents },
                };
        }
    }
}
