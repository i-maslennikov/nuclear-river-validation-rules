using NuClear.CustomerIntelligence.Domain;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.EntityTypes
{
    public sealed class EntityTypeCategoryGroup : EntityTypeBase<EntityTypeCategoryGroup>
    {
        public override int Id
        {
            get { return EntityTypeIds.CategoryGroup; }
        }

        public override string Description
        {
            get { return "CategoryGroup"; }
        }
    }
}