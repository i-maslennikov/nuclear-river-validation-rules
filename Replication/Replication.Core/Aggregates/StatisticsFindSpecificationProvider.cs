using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class StatisticsFindSpecificationProvider<T> : IFindSpecificationProvider<T, RecalculateStatisticsOperation>
        where T : class
    {
        private readonly ValueObjectMetadata<T, StatisticsKey> _metadata;

        public StatisticsFindSpecificationProvider(ValueObjectMetadata<T, StatisticsKey> metadata)
        {
            _metadata = metadata;
        }

        public FindSpecification<T> Create(IEnumerable<RecalculateStatisticsOperation> commands)
        {
            var keys = commands.Select(c => new StatisticsKey { ProjectId = c.ProjectId, CategoryId = c.CategoryId }).ToArray();
            return _metadata.FindSpecificationProvider.Invoke(keys);
        }
    }
}