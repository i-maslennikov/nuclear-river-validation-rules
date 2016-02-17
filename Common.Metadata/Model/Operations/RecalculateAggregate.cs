using NuClear.AdvancedSearch.Common.Metadata.Context;

namespace NuClear.AdvancedSearch.Common.Metadata.Model.Operations
{
    public sealed class RecalculateAggregate : AggregateOperation
    {
        public RecalculateAggregate(Predicate context)
            : base(context)
        {
        }
    }
}