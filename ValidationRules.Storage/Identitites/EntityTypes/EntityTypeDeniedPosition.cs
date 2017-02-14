using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeDeniedPosition : EntityTypeBase<EntityTypeDeniedPosition>
    {
        public override int Id { get; } = EntityTypeIds.DeniedPosition;
        public override string Description { get; } = nameof(EntityTypeIds.DeniedPosition);
    }
}
