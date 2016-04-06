using NuClear.Model.Common.Entities;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class DestroyAggregate : AggregateOperation
    {
        public DestroyAggregate(IEntityType entityType, long entityId)
            : base(entityType, entityId)
        {
        }
    }
}