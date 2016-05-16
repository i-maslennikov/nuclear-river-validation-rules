using System;

using NuClear.Messaging.API;
using NuClear.Replication.Core;

namespace NuClear.Replication.OperationsProcessing
{
    /// <summary>
    /// Transport independent message type
    /// </summary>
    public sealed class EventMessage : MessageBase
    {
        public EventMessage(Guid id, DateTime eventHappenedTime, IEvent @event)
        {
            Id = id;
            Event = @event;
            EventHappenedTime = eventHappenedTime;
        }

        public override Guid Id { get; }

        public DateTime EventHappenedTime { get; }

        public IEvent Event { get; }
    }
}