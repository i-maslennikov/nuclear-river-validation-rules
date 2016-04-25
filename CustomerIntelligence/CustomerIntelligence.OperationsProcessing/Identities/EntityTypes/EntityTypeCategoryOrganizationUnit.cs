using NuClear.CustomerIntelligence.Replication;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeCategoryOrganizationUnit : EntityTypeBase<EntityTypeCategoryOrganizationUnit>
    {
        public override int Id
        {
            get { return EntityTypeIds.CategoryOrganizationUnit; }
        }

        public override string Description
        {
            get { return "CategoryOrganizationUnit"; }
        }
    }
}