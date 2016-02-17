using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeActivity : EntityTypeBase<EntityTypeActivity>
    {
        public override int Id
        {
            get { return EntityTypeIds.Activity; }
        }

        public override string Description
        {
            get { return "Activity"; }
        }
    }
}