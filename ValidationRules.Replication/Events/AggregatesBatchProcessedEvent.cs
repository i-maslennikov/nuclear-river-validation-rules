using System;

using NuClear.Replication.Core;

namespace NuClear.ValidationRules.Replication.Events
{
    public sealed class AggregatesBatchProcessedEvent : IEvent
    {
        public AggregatesBatchProcessedEvent(DateTime eventTime)
        {
            EventTime = eventTime;
        }

        public DateTime EventTime { get; }
    }
}