using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeNomenclatureCategory : EntityTypeBase<EntityTypeNomenclatureCategory>
    {
        public override int Id { get; } = EntityTypeIds.NomenclatureCategory;
        public override string Description { get; } = nameof(EntityTypeIds.NomenclatureCategory);
    }
}