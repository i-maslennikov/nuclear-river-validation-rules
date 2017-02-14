using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Identitites.EntityTypes
{
    public sealed class EntityTypeBranchOfficeOrganizationUnit : EntityTypeBase<EntityTypeBranchOfficeOrganizationUnit>
    {
        public override int Id => EntityTypeIds.BranchOfficeOrganizationUnit;

        public override string Description => nameof(EntityTypeIds.BranchOfficeOrganizationUnit);
    }
}