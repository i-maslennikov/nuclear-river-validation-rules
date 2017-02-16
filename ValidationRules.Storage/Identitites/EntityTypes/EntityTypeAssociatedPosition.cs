using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeAssociatedPosition : EntityTypeBase<EntityTypeAssociatedPosition>
    {
        public override int Id { get; } = EntityTypeIds.AssociatedPosition;
        public override string Description { get; } = nameof(EntityTypeIds.AssociatedPosition);
    }
}
