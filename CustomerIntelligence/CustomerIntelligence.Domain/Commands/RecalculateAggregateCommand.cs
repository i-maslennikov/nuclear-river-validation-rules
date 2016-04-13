using System;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public class RecalculateAggregateCommand : IAggregateCommand
    {
        public RecalculateAggregateCommand(Type aggregateType, long aggregateId)
        {
            AggregateType = aggregateType;
            AggregateId = aggregateId;
        }

        public Type AggregateType { get; }

        public long AggregateId { get; }
    }
}