using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Model.Common.Entities;

namespace NuClear.AdvancedSearch.Replication.Model.EntityTypes
{
    public class EntityTypeFirmCategoryGroup : EntityTypeBase<EntityTypeFirmCategoryGroup>
    {
        public override int Id
        {
            get { return EntityTypeIds.FirmCategoryGroup; }
        }

        public override string Description
        {
            get { return "FirmCategoryGroup"; }
        }
    }
}