using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata;

namespace NuClear.ValidationRules.Domain.EntityTypes
{
    public sealed class EntityTypeAssociatedPositionsGroup : EntityTypeBase<EntityTypeAssociatedPositionsGroup>
    {
        public override int Id { get; } = EntityTypeIds.AssociatedPositionsGroup;
        public override string Description { get; } = nameof(EntityTypeIds.AssociatedPositionsGroup);
    }
}
