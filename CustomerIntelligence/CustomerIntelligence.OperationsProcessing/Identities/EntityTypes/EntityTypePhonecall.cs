using NuClear.CustomerIntelligence.Replication;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypePhonecall : EntityTypeBase<EntityTypePhonecall>
    {
        public override int Id
        {
            get { return EntityTypeIds.Phonecall; }
        }

        public override string Description
        {
            get { return "Phonecall"; }
        }
    }
}