using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class AggregateProcessor<TRootEntity> : IAggregateProcessor
        where TRootEntity : class, IIdentifiable<long>
    {
        private readonly IFindSpecificationProvider<TRootEntity, long> _findSpecificationProvider;
        private readonly EntityProcessor<TRootEntity> _rootEntityProcessor;
        private readonly IReadOnlyCollection<IChildEntityProcessor<TRootEntity>> _childEntityProcessors;

        public AggregateProcessor(IFindSpecificationProvider<TRootEntity, long> findSpecificationProvider, EntityProcessor<TRootEntity> rootEntityProcessor, IReadOnlyCollection<IChildEntityProcessor<TRootEntity>> childEntityProcessors)
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
}