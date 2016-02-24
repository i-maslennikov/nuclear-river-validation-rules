using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeFirmCategory3 : EntityTypeBase<EntityTypeFirmCategory3>
    {
        public override int Id
        {
            get { return EntityTypeIds.FirmCategory3; }
        }

        public override string Description
        {
            get { return "FirmCategory3"; }
        }
    }
}