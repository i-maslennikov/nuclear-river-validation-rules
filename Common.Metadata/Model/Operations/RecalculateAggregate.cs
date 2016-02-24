using System;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class RecalculateAggregate : AggregateOperation
    {
        public RecalculateAggregate(Type aggregateType, long aggregateId)
            : base(aggregateType, aggregateId)
        {
        }
    }
}