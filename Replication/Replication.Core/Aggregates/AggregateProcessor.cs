using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

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
        private readonly IIdentityProvider<long> _aggregateIdentityProvider;
        private readonly Func<IEnumerable<long>, FindSpecification<T>> _findSpecificationProvider;
        private readonly Func<T, long> _identityProvider;
        private readonly EqualityComparer<long> _equalityProvider;

        public AggregateProcessor(AggregateMetadata<T, long> metadata, IValueObjectProcessorFactory valueObjectProcessorFactory, IQuery query, IBulkRepository<T> repository)
        {
            _metadata = metadata;
            _query = query;
            _repository = repository;
            _aggregateChangesDetector = new DataChangesDetector<T, T>(_metadata.MapSpecificationProviderForSource, _metadata.MapSpecificationProviderForTarget, _query);
            _valueObjectProcessors = _metadata.Elements.OfType<IValueObjectMetadataElement>().Select(valueObjectProcessorFactory.Create).ToArray();
            _aggregateIdentityProvider = DefaultIdentityProvider.Instance;
            _findSpecificationProvider = ids => new FindSpecification<T>(_aggregateIdentityProvider.Create<T, long>(ids));
            _identityProvider = _aggregateIdentityProvider.ExtractIdentity<T>().Compile();
            _equalityProvider = EqualityComparer<long>.Default;
        }

        public void Initialize(IReadOnlyCollection<long> ids)
        {
            var mergeResult = _aggregateChangesDetector.DetectChanges(_identityProvider, _findSpecificationProvider.Invoke(ids), _equalityProvider);

            var createFilter = _findSpecificationProvider.Invoke(mergeResult.Difference.ToArray());

            var aggregatesToCreate = _metadata.MapSpecificationProviderForSource.Invoke(createFilter).Map(_query);

            _repository.Create(aggregatesToCreate);

            ApplyChangesToValueObjects(ids);
        }

        public void Recalculate(IReadOnlyCollection<long> ids)
        {
            ApplyChangesToValueObjects(ids);

            var mergeResult = _aggregateChangesDetector.DetectChanges(_identityProvider, _findSpecificationProvider.Invoke(ids), _equalityProvider);

            var createFilter = _findSpecificationProvider.Invoke(mergeResult.Difference.ToArray());
            var updateFilter = _findSpecificationProvider.Invoke(mergeResult.Intersection.ToArray());
            var deleteFilter = _findSpecificationProvider.Invoke(mergeResult.Complement.ToArray());

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

            var mergeResult = _aggregateChangesDetector.DetectChanges(_identityProvider, _findSpecificationProvider.Invoke(ids), _equalityProvider);

            var deleteFilter = _findSpecificationProvider.Invoke(mergeResult.Complement.ToArray());

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