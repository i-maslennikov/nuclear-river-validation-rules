using NuClear.CustomerIntelligence.Replication;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeFirm : EntityTypeBase<EntityTypeFirm>
    {
        public override int Id
        {
            get { return EntityTypeIds.Firm; }
        }

        public override string Description
        {
            get { return "Firm"; }
        }
    }
}