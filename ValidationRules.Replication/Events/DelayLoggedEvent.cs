using System;

using NuClear.Replication.Core;

namespace NuClear.ValidationRules.Replication.Events
{
    public sealed class DelayLoggedEvent : IEvent
    {
        public DelayLoggedEvent(DateTime eventTime)
        {
            EventTime = eventTime;
        }

        public DateTime EventTime { get; }
    }
}