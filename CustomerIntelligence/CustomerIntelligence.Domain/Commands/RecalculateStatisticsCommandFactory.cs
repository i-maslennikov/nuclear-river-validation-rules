using System;

using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public class RecalculateStatisticsCommandFactory : ICommandFactory<StatisticsKey>
    {
        public IOperation Create(Type entityType, StatisticsKey key)
        {
            return new RecalculateStatisticsOperation(key);
        }
    }
}