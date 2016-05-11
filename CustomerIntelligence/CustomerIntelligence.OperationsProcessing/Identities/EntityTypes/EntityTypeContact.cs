using NuClear.CustomerIntelligence.Replication;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeContact : EntityTypeBase<EntityTypeContact>
    {
        public override int Id
        {
            get { return EntityTypeIds.Contact; }
        }

        public override string Description
        {
            get { return "Contact"; }
        }
    }
}