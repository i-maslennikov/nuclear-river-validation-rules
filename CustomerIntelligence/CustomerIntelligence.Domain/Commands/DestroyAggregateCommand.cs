using System;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public class DestroyAggregateCommand : IAggregateCommand
    {
        public DestroyAggregateCommand(Type aggregateType, long aggregateId)
        {
            AggregateType = aggregateType;
            AggregateId = aggregateId;
        }

        public Type AggregateType { get; }

        public long AggregateId { get; }
    }
}