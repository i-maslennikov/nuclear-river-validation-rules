using System;

using NuClear.CustomerIntelligence.Domain.Model;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public class RecalculateStatisticsCommandFactory : ICommandFactory<StatisticsKey>
    {
        public IOperation Create(Type entityType, StatisticsKey key)
        {
            return new RecalculateStatisticsOperation { ProjectId = key.ProjectId, CategoryId = key.CategoryId };
        }
    }
}