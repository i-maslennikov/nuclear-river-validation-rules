using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.CustomerIntelligence.Domain;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
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