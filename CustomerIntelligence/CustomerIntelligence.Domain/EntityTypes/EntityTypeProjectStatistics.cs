using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeProjectStatistics : EntityTypeBase<EntityTypeProjectStatistics>
    {
        public override int Id => EntityTypeIds.ProjectStatistics;

        public override string Description => "ProjectStatistics";
    }
}