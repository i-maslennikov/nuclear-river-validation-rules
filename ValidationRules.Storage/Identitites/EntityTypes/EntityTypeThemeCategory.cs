using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeThemeCategory : EntityTypeBase<EntityTypeThemeCategory>
    {
        public override int Id { get; } = EntityTypeIds.ThemeCategory;
        public override string Description { get; } = nameof(EntityTypeIds.ThemeCategory);
    }
}