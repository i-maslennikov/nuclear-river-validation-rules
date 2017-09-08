using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core;

namespace NuClear.ValidationRules.Replication.Events
{
    public sealed class FlowEvent : IEvent
    {
        public FlowEvent(IMessageFlow flow, IEvent @event)
        {
            Flow = flow;
            Event = @event;
        }

        public IMessageFlow Flow { get; }
        public IEvent Event { get; }
    }
}