using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
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