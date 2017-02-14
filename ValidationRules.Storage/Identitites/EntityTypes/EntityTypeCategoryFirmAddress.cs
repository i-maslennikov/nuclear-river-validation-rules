using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeCategoryFirmAddress : EntityTypeBase<EntityTypeCategoryFirmAddress>
    {
        public override int Id { get; } = EntityTypeIds.CategoryFirmAddress;
        public override string Description { get; } = nameof(EntityTypeIds.CategoryFirmAddress);
    }
}