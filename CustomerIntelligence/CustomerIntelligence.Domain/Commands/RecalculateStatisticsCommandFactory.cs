using NuClear.CustomerIntelligence.Domain.EntityTypes;
using NuClear.Model.Common.Entities;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public class RecalculateStatisticsCommandFactory : ICommandFactory<StatisticsKey>
    {
        public IOperation Create(IEntityType entityType, StatisticsKey key)
        {
            return key.CategoryId.HasValue
                       ? ForProjectCategory(key.ProjectId, key.CategoryId.Value)
                       : ForProject(key.ProjectId);
        }

        private static IOperation ForProjectCategory(long projectId, long categoryId)
            => new RecalculateAggregatePart(EntityTypeProjectStatistics.Instance.Id, projectId, EntityTypeProjectCategoryStatistics.Instance.Id, categoryId);

        private static IOperation ForProject(long projectId)
            => new RecalculateAggregate(EntityTypeProjectStatistics.Instance.Id, projectId);
    }
}