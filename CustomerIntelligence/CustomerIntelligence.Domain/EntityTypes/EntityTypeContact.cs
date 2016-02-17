using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
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