using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeProjectCategory : EntityTypeBase<EntityTypeProjectCategory>
    {
        public override int Id
        {
            get { return EntityTypeIds.ProjectCategory; }
        }

        public override string Description
        {
            get { return "ProjectCategory"; }
        }
    }
}