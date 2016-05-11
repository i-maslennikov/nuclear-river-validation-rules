using System.Collections.Generic;
using System.Transactions;

using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.DataObjects
{
    public class DataChangesDetector<T>
    {
        private readonly MapToObjectsSpecProvider<T, T> _sourceProvider;
        private readonly MapToObjectsSpecProvider<T, T> _targetProvider;
        private readonly IEqualityComparer<T> _comparer;

        public DataChangesDetector(
            MapToObjectsSpecProvider<T, T> sourceProvider,
            MapToObjectsSpecProvider<T, T> targetProvider,
            IEqualityComparer<T> comparer)
        {
            _sourceProvider = sourceProvider;
            _targetProvider = targetProvider;
            _comparer = comparer;
        }

        public MergeResult<T> DetectChanges(FindSpecification<T> specification)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                var sourceObjects = _sourceProvider.Invoke(specification);
                var targetObjects = _targetProvider.Invoke(specification);

                var result = MergeTool.Merge(sourceObjects, targetObjects, _comparer);

                scope.Complete();

                return result;
            }
        }
    }
}