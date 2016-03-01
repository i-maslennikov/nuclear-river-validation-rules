using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata;

namespace NuClear.ValidationRules.Domain.EntityTypes
{
    public sealed class EntityTypeGlobalAssociatedPosition : EntityTypeBase<EntityTypeGlobalAssociatedPosition>
    {
        public override int Id { get; } = EntityTypeIds.GlobalAssociatedPosition;
        public override string Description { get; } = nameof(EntityTypeIds.GlobalAssociatedPosition);
    }
}
