using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class AggregateProcessor<TRootEntity, TRootKey> : IAggregateProcessor
        where TRootEntity : class, IIdentifiable<TRootKey>
    {
        private readonly IFindSpecificationProvider<TRootEntity, TRootKey> _findSpecificationProvider;
        private readonly EntityProcessor<TRootEntity> _rootEntityProcessor;
        private readonly IReadOnlyCollection<IChildEntityProcessor<TRootKey>> _childEntityProcessors;

        public AggregateProcessor(IFindSpecificationProvider<TRootEntity, TRootKey> findSpecificationProvider, EntityProcessor<TRootEntity> rootEntityProcessor, IReadOnlyCollection<IChildEntityProcessor<TRootKey>> childEntityProcessors)
        {
            _findSpecificationProvider = findSpecificationProvider;
            _rootEntityProcessor = rootEntityProcessor;
            _childEntityProcessors = childEntityProcessors;
        }

        public void Initialize(IReadOnlyCollection<InitializeAggregate> commands)
        {
            var keys = commands.Select(x => (TRootKey)x.AggregateRoot.EntityKey).ToArray();
            var specification = _findSpecificationProvider.Create(keys);
            _rootEntityProcessor.Initialize(specification);
            foreach (var processor in _childEntityProcessors)
            {
                processor.Initialize(keys);
            }
        }

        public void Recalculate(IReadOnlyCollection<RecalculateAggregate> commands)
        {
            var keys = commands.Select(x => (TRootKey)x.AggregateRoot.EntityKey).ToArray();
            var specification = _findSpecificationProvider.Create(keys);
            _rootEntityProcessor.Recalculate(specification);
            foreach (var processor in _childEntityProcessors)
            {
                processor.Recalculate(keys);
            }
        }

        public void Recalculate(Type partType, IReadOnlyCollection<RecalculateAggregatePart> commands)
        {
            var keys = commands.GroupBy(x => (TRootKey)x.AggregateRoot.EntityKey, x => x.Entity.EntityKey).ToArray();
            var processor = _childEntityProcessors.Single(x => x.EntityType == partType);
            processor.Recalculate(keys);
        }

        public void Destroy(IReadOnlyCollection<DestroyAggregate> commands)
        {
            var keys = commands.Select(x => (TRootKey)x.AggregateRoot.EntityKey).ToArray();
            var specification = _findSpecificationProvider.Create(keys);
            _rootEntityProcessor.Destroy(specification);
            foreach (var processor in _childEntityProcessors)
            {
                processor.Destroy(keys);
            }
        }
    }
}