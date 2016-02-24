using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class AggregateProcessor<T> : IAggregateProcessor
        where T : class, IIdentifiable<long>
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<T> _repository;
        private readonly AggregateMetadata<T, long> _metadata;
        private readonly DataChangesDetector<T, T> _aggregateChangesDetector;
        private readonly IReadOnlyCollection<IValueObjectProcessor> _valueObjectProcessors;

        public AggregateProcessor(AggregateMetadata<T, long> metadata, IValueObjectProcessorFactory valueObjectProcessorFactory, IQuery query, IBulkRepository<T> repository)
        {
            _metadata = metadata;
            _query = query;
            _repository = repository;
            _aggregateChangesDetector = new DataChangesDetector<T, T>(_metadata.MapSpecificationProviderForSource, _metadata.MapSpecificationProviderForTarget, _query);
            _valueObjectProcessors = _metadata.Elements.OfType<IValueObjectMetadataElement>().Select(valueObjectProcessorFactory.Create).ToArray();
        }

        public void Initialize(IReadOnlyCollection<long> ids)
        {
            var mergeResult = _aggregateChangesDetector.DetectChanges(Specs.Map.ToIds<T>(), _metadata.FindSpecificationProvider.Invoke(ids), EqualityComparer<long>.Default);

            var createFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Difference.ToArray());

            var aggregatesToCreate = _metadata.MapSpecificationProviderForSource.Invoke(createFilter).Map(_query);

            _repository.Create(aggregatesToCreate);

            ApplyChangesToValueObjects(ids);
        }

        public void Recalculate(IReadOnlyCollection<long> ids)
        {
            ApplyChangesToValueObjects(ids);

            var mergeResult = _aggregateChangesDetector.DetectChanges(Specs.Map.ToIds<T>(), _metadata.FindSpecificationProvider.Invoke(ids), EqualityComparer<long>.Default);

            var createFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Difference.ToArray());
            var updateFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Intersection.ToArray());
            var deleteFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Complement.ToArray());

            var aggregatesToCreate = _metadata.MapSpecificationProviderForSource.Invoke(createFilter).Map(_query).ToArray();
            var aggregatesToUpdate = _metadata.MapSpecificationProviderForSource.Invoke(updateFilter).Map(_query).ToArray();
            var aggregatesToDelete = _metadata.MapSpecificationProviderForTarget.Invoke(deleteFilter).Map(_query).ToArray();

            _repository.Delete(aggregatesToDelete);
            _repository.Create(aggregatesToCreate);
            _repository.Update(aggregatesToUpdate);
        }

        public void Destroy(IReadOnlyCollection<long> ids)
        {
            ApplyChangesToValueObjects(ids);

            var mergeResult = _aggregateChangesDetector.DetectChanges(Specs.Map.ToIds<T>(), _metadata.FindSpecificationProvider.Invoke(ids), EqualityComparer<long>.Default);

            var deleteFilter = _metadata.FindSpecificationProvider.Invoke(mergeResult.Complement.ToArray());

            var aggregatesToDelete = _metadata.MapSpecificationProviderForTarget.Invoke(deleteFilter).Map(_query);

            _repository.Delete(aggregatesToDelete);
        }

        private void ApplyChangesToValueObjects(IReadOnlyCollection<long> aggregateIds)
        {
            foreach (var processor in _valueObjectProcessors)
            {
                processor.ApplyChanges(aggregateIds);
            }
        }
    }
}