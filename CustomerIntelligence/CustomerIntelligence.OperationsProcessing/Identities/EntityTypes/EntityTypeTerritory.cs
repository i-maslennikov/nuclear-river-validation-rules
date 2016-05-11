using NuClear.CustomerIntelligence.Replication;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeTerritory : EntityTypeBase<EntityTypeTerritory>
    {
        public override int Id
        {
            get { return EntityTypeIds.Territory; }
        }

        public override string Description
        {
            get { return "Territory"; }
        }
    }
}