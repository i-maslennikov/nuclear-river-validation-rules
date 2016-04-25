using NuClear.CustomerIntelligence.Domain;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.EntityTypes
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