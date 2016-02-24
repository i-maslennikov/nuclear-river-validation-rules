using NuClear.River.Common.Metadata.Context;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class RecalculateStatisticsOperation : AggregateOperation
    {
        public RecalculateStatisticsOperation(Predicate context)
            : base(context)
        {
        }
    }
}
