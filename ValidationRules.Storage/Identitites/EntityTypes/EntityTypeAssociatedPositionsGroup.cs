using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeAssociatedPositionsGroup : EntityTypeBase<EntityTypeAssociatedPositionsGroup>
    {
        public override int Id { get; } = EntityTypeIds.AssociatedPositionsGroup;
        public override string Description { get; } = nameof(EntityTypeIds.AssociatedPositionsGroup);
    }
}
