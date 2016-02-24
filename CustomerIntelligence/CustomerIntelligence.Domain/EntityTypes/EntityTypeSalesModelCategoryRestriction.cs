using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public class EntityTypeSalesModelCategoryRestriction : EntityTypeBase<EntityTypeSalesModelCategoryRestriction>
    {
        public override int Id
        {
            get { return EntityTypeIds.SalesModelCategoryRestriction; }
        }

        public override string Description
        {
            get { return "SalesModelCategoryRestriction"; }
        }
    }
}