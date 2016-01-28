using NuClear.AdvancedSearch.Common.Metadata.Context;

namespace NuClear.AdvancedSearch.Common.Metadata.Model.Operations
{
    public sealed class InitializeAggregate : AggregateOperation
    {
        public InitializeAggregate(Predicate context)
            : base(context)
        {
        }
    }
}