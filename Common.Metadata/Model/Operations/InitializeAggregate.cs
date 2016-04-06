using NuClear.Model.Common.Entities;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class InitializeAggregate : AggregateOperation
    {
        public InitializeAggregate(IEntityType entityType, long entityId)
            : base(entityType, entityId)
        {
        }
    }
}