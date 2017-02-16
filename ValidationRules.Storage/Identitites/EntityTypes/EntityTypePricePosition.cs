using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypePricePosition : EntityTypeBase<EntityTypePricePosition>
    {
        public override int Id { get; } = EntityTypeIds.PricePosition;
        public override string Description { get; } = nameof(EntityTypeIds.PricePosition);
    }
}
