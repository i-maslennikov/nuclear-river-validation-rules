using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeFirmCategory1 : EntityTypeBase<EntityTypeFirmCategory1>
    {
        public override int Id
        {
            get { return EntityTypeIds.FirmCategory1; }
        }

        public override string Description
        {
            get { return "FirmCategory1"; }
        }
    }
}