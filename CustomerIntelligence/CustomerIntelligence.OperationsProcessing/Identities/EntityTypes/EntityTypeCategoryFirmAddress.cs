using NuClear.CustomerIntelligence.Replication;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeCategoryFirmAddress : EntityTypeBase<EntityTypeCategoryFirmAddress>
    {
        public override int Id
        {
            get { return EntityTypeIds.CategoryFirmAddress; }
        }

        public override string Description
        {
            get { return "CategoryFirmAddress"; }
        }
    }
}