using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeTask : EntityTypeBase<EntityTypeTask>
    {
        public override int Id
        {
            get { return EntityTypeIds.Task; }
        }

        public override string Description
        {
            get { return "Task"; }
        }
    }
}