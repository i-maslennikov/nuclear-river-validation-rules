using NuClear.Model.Common.Entities;

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