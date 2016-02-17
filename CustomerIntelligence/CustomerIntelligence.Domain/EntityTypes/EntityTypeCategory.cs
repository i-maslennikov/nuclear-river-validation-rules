using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeCategory : EntityTypeBase<EntityTypeCategory>
    {
        public override int Id
        {
            get { return EntityTypeIds.Category; }
        }

        public override string Description
        {
            get { return "Category"; }
        }
    }
}