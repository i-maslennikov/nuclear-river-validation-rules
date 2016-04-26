using System;

using NuClear.Replication.Core.Commands;

namespace NuClear.CustomerIntelligence.Replication.Commands
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