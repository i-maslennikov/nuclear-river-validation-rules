using NuClear.AdvancedSearch.Common.Metadata.Context;

namespace NuClear.AdvancedSearch.Common.Metadata.Model.Operations
{
    public sealed class RecalculateStatisticsOperation : AggregateOperation
    {
        public RecalculateStatisticsOperation(Predicate context)
            : base(context)
        {
        }
    }
}
