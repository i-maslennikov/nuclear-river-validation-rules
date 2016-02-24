using NuClear.River.Common.Metadata.Context;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class DestroyAggregate : AggregateOperation
    {
        public DestroyAggregate(Predicate context)
            : base(context)
        {
        }
    }
}