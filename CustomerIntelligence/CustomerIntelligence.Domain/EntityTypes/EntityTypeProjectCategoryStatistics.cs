using NuClear.Model.Common.Entities;

namespace NuClear.CustomerIntelligence.Domain.EntityTypes
{
    public sealed class EntityTypeProjectCategoryStatistics : EntityTypeBase<EntityTypeProjectCategoryStatistics>
    {
        public override int Id => EntityTypeIds.ProjectCategoryStatistics;

        public override string Description => "ProjectCategoryStatistics";
    }
}