using NuClear.CustomerIntelligence.Replication;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeProject : EntityTypeBase<EntityTypeProject>
    {
        public override int Id
        {
            get { return EntityTypeIds.Project; }
        }

        public override string Description
        {
            get { return "Project"; }
        }
    }
}