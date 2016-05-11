using NuClear.CustomerIntelligence.Replication;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeLetter : EntityTypeBase<EntityTypeLetter>
    {
        public override int Id
        {
            get { return EntityTypeIds.Letter; }
        }

        public override string Description
        {
            get { return "Letter"; }
        }
    }
}