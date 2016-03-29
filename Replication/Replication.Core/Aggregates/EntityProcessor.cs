using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class EntityProcessor<TEntity> : IEntityProcessor<TEntity>
    {
        private readonly IBulkRepository<TEntity> _repository;
        private readonly DataChangesDetector<TEntity> _aggregateChangesDetector;
        private readonly IReadOnlyCollection<IValueObjectProcessor<TEntity>> _valueObjectProcessors;

        public EntityProcessor(IBulkRepository<TEntity> repository, DataChangesDetector<TEntity> aggregateChangesDetector, IReadOnlyCollection<IValueObjectProcessor<TEntity>> valueObjectProcessors)
        {
            _repository = repository;
            _aggregateChangesDetector = aggregateChangesDetector;
            _valueObjectProcessors = valueObjectProcessors;
        }

        public void Initialize(FindSpecification<TEntity> specification)
        {
            var mergeResult = _aggregateChangesDetector.DetectChanges(specification);

            _repository.Create(mergeResult.Difference);

            ApplyChangesToValueObjects(mergeResult.Difference.ToArray());
        }

        public void Recalculate(FindSpecification<TEntity> specification)
        {
            var mergeResult = _aggregateChangesDetector.DetectChanges(specification);

            _repository.Delete(mergeResult.Complement);
            _repository.Create(mergeResult.Difference);
            _repository.Update(mergeResult.Intersection);

            ApplyChangesToValueObjects(mergeResult.Complement.ToArray());
            ApplyChangesToValueObjects(mergeResult.Difference.ToArray());
            ApplyChangesToValueObjects(mergeResult.Intersection.ToArray());
        }

        public void Destroy(FindSpecification<TEntity> specification)
        {
            var mergeResult = _aggregateChangesDetector.DetectChanges(specification);

            _repository.Delete(mergeResult.Complement);

            ApplyChangesToValueObjects(mergeResult.Complement.ToArray());
        }

        private void ApplyChangesToValueObjects(IReadOnlyCollection<TEntity> changes)
        {
            foreach (var processor in _valueObjectProcessors)
            {
                processor.Execute(changes);
            }
        }
    }
}