using System;
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

        public MergeResult<TKey> DetectChanges<TKey>(Func<TOutput, TKey> mapping, FindSpecification<TFilter> specification, IEqualityComparer<TKey> comparer)
        {
            // Заметка: и сейчас, без mapSecification, и ранее идентификаторы извлекались из материализованных сущностей.
            // Можно взять задачу на оптимизацию поведения - не выбирать из БД избыточные данные, материализовывать не TOutput, а сразу TKey
            // Для этого потребуется переделать MapToObjectsSpecProvider так, чтобы в нйм вместо IEnumerable был IQueryable и использовать не делегат, а выражение.
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                var sourceObjects = _sourceProvider.Invoke(specification).Map(_query);
                var targetObjects = _targetProvider.Invoke(specification).Map(_query);

                var result = MergeTool.Merge(
                    sourceObjects.Select(mapping).ToArray(),
                    targetObjects.Select(mapping).ToArray(),
                    comparer);

                scope.Complete();

                return result;
            }
        }
    }
}