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
                       ? new RecalculateStatisticsOperation(key.ProjectId, key.CategoryId.Value)
                       : new RecalculateStatisticsOperation(key.ProjectId);
        }
    }
}