using System.Collections.Generic;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Equality;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.Aggregates
{
    public class StatisticsProcessor<T> : IStatisticsProcessor
        where T : class
    {
        private readonly IBulkRepository<T> _repository;
        private readonly DataChangesDetector<T> _changesDetector;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IFindSpecificationProvider<T, RecalculateStatisticsOperation> _findSpecificationProvider;

        public StatisticsProcessor(DataChangesDetector<T> changesDetector, IBulkRepository<T> repository, IEqualityComparerFactory equalityComparerFactory, IFindSpecificationProvider<T, RecalculateStatisticsOperation> findSpecificationProvider)
        {
            _repository = repository;
            _equalityComparerFactory = equalityComparerFactory;
            _findSpecificationProvider = findSpecificationProvider;
            _changesDetector = changesDetector;
        }

        public void Execute(IReadOnlyCollection<RecalculateStatisticsOperation> commands)
        {
            var filter = _findSpecificationProvider.Create(commands);

            // Сначала сравниением получаем различающиеся записи,
            // затем получаем те из различающихся, которые совпадают по идентификатору.
            var intermediateResult = _changesDetector.DetectChanges(filter);
            var changes = MergeTool.Merge(intermediateResult.Difference, intermediateResult.Complement, _equalityComparerFactory.CreateIdentityComparer<T>());

            _repository.Delete(changes.Complement);
            _repository.Create(changes.Difference);
            _repository.Update(changes.Intersection);
        }
    }
}