using System;

namespace NuClear.CustomerIntelligence.Domain.Commands
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