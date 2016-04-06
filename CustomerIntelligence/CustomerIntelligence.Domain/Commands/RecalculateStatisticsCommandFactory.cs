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
            return ForProjectCategory(key.ProjectId, key.CategoryId);
        }

        private static IOperation ForProjectCategory(long projectId, long categoryId)
            => new RecalculateAggregatePart(EntityTypeProjectStatistics.Instance, projectId, EntityTypeProjectCategoryStatistics.Instance, categoryId);
    }
}