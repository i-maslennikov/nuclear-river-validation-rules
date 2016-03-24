using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class StatisticsFindSpecificationProvider<T> : IFindSpecificationProvider<T, RecalculateAggregatePart>
        where T : class
    {
        private readonly ValueObjectMetadata<T, StatisticsKey> _metadata;

        public StatisticsFindSpecificationProvider(ValueObjectMetadata<T, StatisticsKey> metadata)
        {
            _metadata = metadata;
        }

        public FindSpecification<T> Create(IEnumerable<RecalculateAggregatePart> commands)
        {
            var keys = commands.Select(c => new StatisticsKey { ProjectId = c.AggregateInstanceId, CategoryId = c.EntityInstanceId }).ToArray();
            return _metadata.FindSpecificationProvider.Invoke(keys);
        }
    }
}