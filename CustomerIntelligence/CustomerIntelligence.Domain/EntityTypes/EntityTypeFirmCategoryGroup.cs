using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeFirmCategoryGroup : EntityTypeBase<EntityTypeFirmCategoryGroup>
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