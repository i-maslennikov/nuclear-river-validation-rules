using NuClear.CustomerIntelligence.Replication;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeBranchOfficeOrganizationUnit : EntityTypeBase<EntityTypeBranchOfficeOrganizationUnit>
    {
        public override int Id
        {
            get { return EntityTypeIds.BranchOfficeOrganizationUnit; }
        }

        public override string Description
        {
            get { return "BranchOfficeOrganizationUnit"; }
        }
    }
}