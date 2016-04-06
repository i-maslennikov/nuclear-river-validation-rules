using NuClear.Model.Common.Entities;

namespace NuClear.River.Common.Metadata.Model.Operations
{
    public sealed class RecalculateAggregate : AggregateOperation
    {
        public RecalculateAggregate(IEntityType entityType, long entityId)
            : base(entityType, entityId)
        {
        }
    }
}