using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Replication;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeBranchOfficeOrganizationUnit : EntityTypeBase<EntityTypeBranchOfficeOrganizationUnit>
    {
        public override int Id => EntityTypeIds.BranchOfficeOrganizationUnit;

        public override string Description => nameof(EntityTypeIds.BranchOfficeOrganizationUnit);
    }
}