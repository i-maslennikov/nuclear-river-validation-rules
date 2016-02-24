using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Equality;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.Aggregates
{
    public class StatisticsProcessor<T> : IStatisticsProcessor
        where T : class
    {
        private readonly IBulkRepository<T> _repository;
        private readonly StatisticsRecalculationMetadata<T> _metadata;
        private readonly DataChangesDetector<T, T> _changesDetector;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public StatisticsProcessor(StatisticsRecalculationMetadata<T> metadata, IQuery query, IBulkRepository<T> repository, IEqualityComparerFactory equalityComparerFactory)
        {
            _repository = repository;
            _metadata = metadata;
            _equalityComparerFactory = equalityComparerFactory;
            _changesDetector = new DataChangesDetector<T, T>(_metadata.MapSpecificationProviderForSource, _metadata.MapSpecificationProviderForTarget, query);
        }

        public void RecalculateStatistics(StatisticsProcessorSlice slice)
        {
            // todo: имеет ли смысл slice передать в спецификацию?
            var filter = _metadata.FindSpecificationProvider.Invoke(slice.ProjectId, slice.CategoryIds);

            // Сначала сравниением получаем различающиеся записи,
            // затем получаем те из различающихся, которые совпадают по идентификатору.
            var intermediateResult = _changesDetector.DetectChanges(Specs.Map.ZeroMapping<T>(), filter, _equalityComparerFactory.CreateCompleteComparer<T>());
            var changes = MergeTool.Merge(intermediateResult.Difference, intermediateResult.Complement, _equalityComparerFactory.CreateIdentityComparer<T>());

            _repository.Delete(changes.Complement);
            _repository.Create(changes.Difference);
            _repository.Update(changes.Intersection);
        }
    }
}