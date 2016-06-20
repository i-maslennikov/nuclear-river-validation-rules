using NuClear.CustomerIntelligence.Replication;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.EntityTypes
{
    public sealed class EntityTypeLead : EntityTypeBase<EntityTypeLead>
    {
        public override int Id => EntityTypeIds.Lead;

        public override string Description => "Lead";
    }
}