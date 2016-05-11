using NuClear.CustomerIntelligence.Replication;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeAccount : EntityTypeBase<EntityTypeAccount>
    {
        public override int Id
        {
            get { return EntityTypeIds.Account; }
        }

        public override string Description
        {
            get { return "Account"; }
        }
    }
}