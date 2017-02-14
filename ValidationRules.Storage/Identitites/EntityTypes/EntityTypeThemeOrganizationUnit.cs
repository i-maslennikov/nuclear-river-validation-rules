using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeThemeOrganizationUnit : EntityTypeBase<EntityTypeThemeOrganizationUnit>
    {
        public override int Id { get; } = EntityTypeIds.ThemeOrganizationUnit;
        public override string Description { get; } = nameof(EntityTypeIds.ThemeOrganizationUnit);
    }
}