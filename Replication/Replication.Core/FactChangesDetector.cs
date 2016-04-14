using System.Collections.Generic;
using System.Transactions;

using NuClear.Replication.Core.API;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core
{
    public class FactChangesDetector<T>
    {
        private readonly MapToObjectsSpecProvider<T, T> _sourceProvider;
        private readonly MapToObjectsSpecProvider<T, T> _targetProvider;
        private readonly IEqualityComparer<T> _identityComparer;
        private readonly IEqualityComparer<T> _completeComparer;
        private readonly IQuery _query;

        public FactChangesDetector(
            MapToObjectsSpecProvider<T, T> sourceProvider,
            MapToObjectsSpecProvider<T, T> targetProvider,
            IEqualityComparer<T> identityComparer,
            IEqualityComparer<T> completeComparer,
            IQuery query)
        {
            _sourceProvider = sourceProvider;
            _targetProvider = targetProvider;
            _query = query;
            _identityComparer = identityComparer;
            _completeComparer = completeComparer;
        }

        public MergeResult<T> DetectChanges(FindSpecification<T> specification)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                var sourceObjects = _sourceProvider.Invoke(specification).Map(_query);
                var targetObjects = _targetProvider.Invoke(specification).Map(_query);

                // Intersection - не изменились совсем
                // Difference - добавленные и изменившиеся
                // Complement - удалённые и изменившиеся
                var preresult = MergeTool.Merge(sourceObjects, targetObjects, _completeComparer);

                var result = MergeTool.Merge(preresult.Difference, preresult.Complement, _identityComparer);

                scope.Complete();

                return result;
            }
        }
    }
}