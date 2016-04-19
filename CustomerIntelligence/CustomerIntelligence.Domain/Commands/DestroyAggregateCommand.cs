using System;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public class DestroyAggregateCommand : IAggregateCommand
    {
        public DestroyAggregateCommand(Type aggregateRootType, long aggregateRootId)
        {
            AggregateRootType = aggregateRootType;
            AggregateRootId = aggregateRootId;
        }

        public Type AggregateRootType { get; }

        public long AggregateRootId { get; }
    }
}