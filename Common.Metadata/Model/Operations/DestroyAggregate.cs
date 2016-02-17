using NuClear.AdvancedSearch.Common.Metadata.Context;

namespace NuClear.AdvancedSearch.Common.Metadata.Model.Operations
{
    public sealed class DestroyAggregate : AggregateOperation
    {
        public DestroyAggregate(Predicate context)
            : base(context)
        {
        }
    }
}