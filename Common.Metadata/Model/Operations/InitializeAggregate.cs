using NuClear.River.Common.Metadata.Context;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class InitializeAggregate : AggregateOperation
    {
        public InitializeAggregate(Predicate context)
            : base(context)
        {
        }
    }
}