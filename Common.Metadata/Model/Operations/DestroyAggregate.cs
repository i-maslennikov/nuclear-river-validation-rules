using System;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class DestroyAggregate : AggregateOperation
    {
        public DestroyAggregate(Type aggregateType, long aggregateId)
            : base(aggregateType, aggregateId)
        {
        }
    }
}