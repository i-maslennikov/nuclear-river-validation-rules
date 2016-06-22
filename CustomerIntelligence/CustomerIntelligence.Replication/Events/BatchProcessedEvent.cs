using System;

using NuClear.Replication.Core;

namespace NuClear.CustomerIntelligence.Replication.Events
{
    public class BatchProcessedEvent : IEvent
    {
        public BatchProcessedEvent(DateTime eventTime)
        {
            EventTime = eventTime;
        }

        public DateTime EventTime { get; }
    }
}