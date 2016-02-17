using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeFirmContact : EntityTypeBase<EntityTypeFirmContact>
    {
        public override int Id
        {
            get { return EntityTypeIds.FirmContact; }
        }

        public override string Description
        {
            get { return "FirmContact"; }
        }
    }
}