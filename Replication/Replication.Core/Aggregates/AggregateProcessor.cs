using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class AggregateProcessor<T> : IAggregateProcessor
        where T : class, IIdentifiable<long>
    {
        private readonly IFindSpecificationProvider<T, long> _findSpecificationProvider;
        private readonly EntityProcessor<T> _rootEntityProcessor;
        private readonly IReadOnlyCollection<IChildEntityProcessor<T>> _childEntityProcessors;

        public AggregateProcessor(IFindSpecificationProvider<T, long> findSpecificationProvider, EntityProcessor<T> rootEntityProcessor, IReadOnlyCollection<IChildEntityProcessor<T>> childEntityProcessors)
        {
            _findSpecificationProvider = findSpecificationProvider;
            _rootEntityProcessor = rootEntityProcessor;
            _childEntityProcessors = childEntityProcessors;
        }

        public void Initialize(IReadOnlyCollection<InitializeAggregate> commands)
        {
            var specification = _findSpecificationProvider.Create(commands.Select(x => x.EntityId));
            _rootEntityProcessor.Initialize(specification);
            foreach (var processor in _childEntityProcessors)
            {
                processor.Initialize(specification);
            }
        }

        public void Recalculate(IReadOnlyCollection<RecalculateAggregate> commands)
        {
            var specification = _findSpecificationProvider.Create(commands.Select(x => x.EntityId));
            _rootEntityProcessor.Recalculate(specification);
            foreach (var processor in _childEntityProcessors)
            {
                processor.Recalculate(specification);
            }
        }

        public void RecalculatePartially(IReadOnlyCollection<RecalculateAggregatePart> commands, Type partType)
        {
            var specification = _findSpecificationProvider.Create(commands.Select(x => x.AggregateInstanceId));
            var processor = _childEntityProcessors.Single(x => x.ChildEntityType == partType);
            processor.RecalculatePartially(specification, commands);
        }

        public void Destroy(IReadOnlyCollection<DestroyAggregate> commands)
        {
            var specification = _findSpecificationProvider.Create(commands.Select(x => x.EntityId));
            _rootEntityProcessor.Destroy(specification);
            foreach (var processor in _childEntityProcessors)
            {
                processor.Destroy(specification);
            }
        }
    }

    public interface IChildEntityProcessor<TRootEntity>
    {
        Type ChildEntityType { get; }
        void Initialize(FindSpecification<TRootEntity> specification);
        void Recalculate(FindSpecification<TRootEntity> specification);
        void Destroy(FindSpecification<TRootEntity> specification);
        void RecalculatePartially(FindSpecification<TRootEntity> specification, IReadOnlyCollection<RecalculateAggregatePart> commands);
    }

    public sealed class ChildEntityProcessor<TRootEntity, TChildEntity> : IChildEntityProcessor<TRootEntity>
    {
        private readonly IEntityProcessor<TChildEntity> _childEntity;
        private readonly IFindSpecificationProvider<TChildEntity, long> _findSpecificationProvider;
        private readonly IMapSpecification<FindSpecification<TRootEntity>, FindSpecification<TChildEntity>> _mapSpecification;

        public Type ChildEntityType
            => typeof(TChildEntity);

        public ChildEntityProcessor(IEntityProcessor<TChildEntity> childEntity,
                                    IFindSpecificationProvider<TChildEntity, long> findSpecificationProvider,
                                    IMapSpecification<FindSpecification<TRootEntity>, FindSpecification<TChildEntity>> mapSpecification)
        {
            _childEntity = childEntity;
            _findSpecificationProvider = findSpecificationProvider;
            _mapSpecification = mapSpecification;
        }

        public void Initialize(FindSpecification<TRootEntity> specification)
        {
            var spec = _mapSpecification.Map(specification);
            _childEntity.Initialize(spec);
        }

        public void Recalculate(FindSpecification<TRootEntity> specification)
        {
            var spec = _mapSpecification.Map(specification);
            _childEntity.Recalculate(spec);
        }

        public void Destroy(FindSpecification<TRootEntity> specification)
        {
            var spec = _mapSpecification.Map(specification);
            _childEntity.Destroy(spec);
        }

        public void RecalculatePartially(FindSpecification<TRootEntity> specification, IReadOnlyCollection<RecalculateAggregatePart> commands)
        {
            var specFromRoot = _mapSpecification.Map(specification);
            var specFromCommands = _findSpecificationProvider.Create(commands.Select(x => x.EntityInstanceId));
            _childEntity.Recalculate(specFromRoot & specFromCommands);
        }
    }

    public interface IEntityProcessor<TEntity>
    {
        void Initialize(FindSpecification<TEntity> specification);
        void Recalculate(FindSpecification<TEntity> specification);
        void Destroy(FindSpecification<TEntity> specification);
    }

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