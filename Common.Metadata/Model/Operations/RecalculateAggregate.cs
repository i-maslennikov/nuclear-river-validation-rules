using NuClear.River.Common.Metadata.Context;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class RecalculateAggregate : AggregateOperation
    {
        public RecalculateAggregate(Predicate context)
            : base(context)
        {
        }
    }
}