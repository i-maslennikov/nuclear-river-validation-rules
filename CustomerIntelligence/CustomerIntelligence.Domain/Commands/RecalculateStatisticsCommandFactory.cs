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
            return ForProjectCategory(key);
        }

        private static IOperation ForProjectCategory(StatisticsKey key)
            => new RecalculateAggregatePart(new EntityReference(EntityTypeProjectStatistics.Instance, key.ProjectId),
                                            new EntityReference(EntityTypeProjectCategoryStatistics.Instance, key));
    }
}