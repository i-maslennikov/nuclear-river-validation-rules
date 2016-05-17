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
        public EventMessage(Guid id, IEvent @event)
        {
            Id = id;
            Event = @event;
        }

        public override Guid Id { get; }

        public IEvent Event { get; }
    }
}