using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeCategoryOrganizationUnit : EntityTypeBase<EntityTypeCategoryOrganizationUnit>
    {
        public override int Id { get; } = EntityTypeIds.CategoryOrganizationUnit;
        public override string Description { get; } = nameof(EntityTypeIds.CategoryOrganizationUnit);
    }
}