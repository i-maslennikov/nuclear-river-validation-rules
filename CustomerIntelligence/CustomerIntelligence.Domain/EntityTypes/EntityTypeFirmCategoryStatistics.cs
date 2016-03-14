using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeFirmCategoryStatistics : EntityTypeBase<EntityTypeFirmCategoryStatistics>
    {
        public override int Id
        {
            get { return EntityTypeIds.FirmCategoryStatistics; }
        }

        public override string Description
        {
            get { return "FirmCategoryStatistics"; }
        }
    }
}