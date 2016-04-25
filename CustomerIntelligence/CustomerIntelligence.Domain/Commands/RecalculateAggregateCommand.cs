using System;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public class RecalculateAggregateCommand : IAggregateCommand
    {
        public RecalculateAggregateCommand(Type aggregateRootType, long aggregateRootId)
        {
            AggregateRootType = aggregateRootType;
            AggregateRootId = aggregateRootId;
        }

        public Type AggregateRootType { get; }

        public long AggregateRootId { get; }
    }
}