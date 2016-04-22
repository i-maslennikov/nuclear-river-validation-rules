using System.Collections.Generic;
using System.Transactions;

using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.API.DataObjects
{
    public class DataChangesDetector<T>
    {
        private readonly MapToObjectsSpecProvider<T, T> _sourceProvider;
        private readonly MapToObjectsSpecProvider<T, T> _targetProvider;
        private readonly IEqualityComparer<T> _comparer;
        private readonly IQuery _query;

        public DataChangesDetector(
            MapToObjectsSpecProvider<T, T> sourceProvider,
            MapToObjectsSpecProvider<T, T> targetProvider,
            IEqualityComparer<T> comparer,
            IQuery query)
        {
            _sourceProvider = sourceProvider;
            _targetProvider = targetProvider;
            _comparer = comparer;
            _query = query;
        }

        public MergeResult<T> DetectChanges(FindSpecification<T> specification)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                var sourceObjects = _sourceProvider.Invoke(specification).Map(_query);
                var targetObjects = _targetProvider.Invoke(specification).Map(_query);

                var result = MergeTool.Merge(sourceObjects, targetObjects, _comparer);

                scope.Complete();

                return result;
            }
        }
    }
}