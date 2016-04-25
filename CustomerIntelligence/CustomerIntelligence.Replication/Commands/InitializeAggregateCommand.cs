using System;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public class InitializeAggregateCommand : IAggregateCommand
    {
        public InitializeAggregateCommand(Type aggregateRootType, long aggregateRootId)
        {
            AggregateRootType = aggregateRootType;
            AggregateRootId = aggregateRootId;
        }

        public Type AggregateRootType { get; }

        public long AggregateRootId { get; }
    }
}