using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Replication.Core.API;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core
{
    public class DataChangesDetector<TFilter, TOutput>
    {
        private readonly IQuery _query;
        private readonly MapToObjectsSpecProvider<TFilter, TOutput> _sourceProvider;
        private readonly MapToObjectsSpecProvider<TFilter, TOutput> _targetProvider;

        public DataChangesDetector(
            MapToObjectsSpecProvider<TFilter, TOutput> sourceProvider,
            MapToObjectsSpecProvider<TFilter, TOutput> targetProvider,
            IQuery query)
        {
            _sourceProvider = sourceProvider;
            _targetProvider = targetProvider;
            _query = query;
        }

        public MergeResult<TCompared> DetectChanges<TCompared>(MapSpecification<IEnumerable<TOutput>, IEnumerable<TCompared>> mapSpec, FindSpecification<TFilter> specification, IEqualityComparer<TCompared> comparer)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                var sourceObjects = _sourceProvider.Invoke(specification).Map(_query);
                var targetObjects = _targetProvider.Invoke(specification).Map(_query);

                var result = MergeTool.Merge(
                    mapSpec.Map(sourceObjects).ToArray(),
                    mapSpec.Map(targetObjects).ToArray(), 
                    comparer);

                scope.Complete();

                return result;
            }
        }
    }
}