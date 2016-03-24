using System;
using System.Collections.Generic;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class AggregateProcessor<T, TKey> : IAggregateProcessor
        where T : class, IIdentifiable<TKey>
    {
        private readonly IBulkRepository<T> _repository;
        private readonly DataChangesDetector<T> _aggregateChangesDetector;
        private readonly IReadOnlyCollection<IValueObjectProcessor> _valueObjectProcessors;
        private readonly IFindSpecificationProvider<T, AggregateOperation> _findSpecificationProvider;

        public AggregateProcessor(DataChangesDetector<T> aggregateChangesDetector, IBulkRepository<T> repository, IReadOnlyCollection<IValueObjectProcessor> valueObjectProcessors, IFindSpecificationProvider<T, AggregateOperation> findSpecificationProvider)
        {
            _repository = repository;
            _aggregateChangesDetector = aggregateChangesDetector;
            _valueObjectProcessors = valueObjectProcessors;
            _findSpecificationProvider = findSpecificationProvider;
        }

        public void Initialize(IReadOnlyCollection<InitializeAggregate> commands)
        {
            var specification = _findSpecificationProvider.Create(commands);
            var mergeResult = _aggregateChangesDetector.DetectChanges(specification);

            _repository.Create(mergeResult.Difference);

            ApplyChangesToValueObjects(commands);
        }

        public void Recalculate(IReadOnlyCollection<RecalculateAggregate> commands)
        {
            ApplyChangesToValueObjects(commands);

            var specification = _findSpecificationProvider.Create(commands);
            var mergeResult = _aggregateChangesDetector.DetectChanges(specification);

            _repository.Delete(mergeResult.Complement);
            _repository.Create(mergeResult.Difference);
            _repository.Update(mergeResult.Intersection);
        }

        public void Recalculate(IReadOnlyCollection<RecalculateAggregatePart> commands)
        {
            throw new NotImplementedException();
        }

        public void Destroy(IReadOnlyCollection<DestroyAggregate> commands)
        {
            ApplyChangesToValueObjects(commands);

            var specification = _findSpecificationProvider.Create(commands);
            var mergeResult = _aggregateChangesDetector.DetectChanges(specification);

            _repository.Delete(mergeResult.Complement);
        }

        private void ApplyChangesToValueObjects(IReadOnlyCollection<AggregateOperation> commands)
        {
            foreach (var processor in _valueObjectProcessors)
            {
                processor.Execute(commands);
            }
        }
    }
}