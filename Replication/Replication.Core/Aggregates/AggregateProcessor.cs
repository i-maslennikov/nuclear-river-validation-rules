using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class AggregateProcessor<TRootEntity> : IAggregateProcessor // ICommandHandler<InitializeAggregate>, ICommandHandler<RecalculateAggregate>, ICommandHandler<DestroyAggregate>, ...
        where TRootEntity : class, IIdentifiable<long>
    {
        private readonly IFindSpecificationProvider<TRootEntity, long> _findSpecificationProvider;
        private readonly EntityProcessor<TRootEntity> _rootEntityProcessor;
        private readonly IReadOnlyCollection<IChildEntityProcessor<long>> _childEntityProcessors;

        public AggregateProcessor(IFindSpecificationProvider<TRootEntity, long> findSpecificationProvider, EntityProcessor<TRootEntity> rootEntityProcessor, IReadOnlyCollection<IChildEntityProcessor<long>> childEntityProcessors)
        {
            _findSpecificationProvider = findSpecificationProvider;
            _rootEntityProcessor = rootEntityProcessor;
            _childEntityProcessors = childEntityProcessors;
        }

        public void Initialize(IReadOnlyCollection<InitializeAggregate> commands)
        {
            var keys = commands.Select(x => x.EntityId).ToArray();
            var specification = _findSpecificationProvider.Create(keys);
            _rootEntityProcessor.Initialize(specification);
            foreach (var processor in _childEntityProcessors)
            {
                processor.Initialize(keys);
            }
        }

        public void Recalculate(IReadOnlyCollection<RecalculateAggregate> commands)
        {
            var keys = commands.Select(x => x.EntityId).ToArray();
            var specification = _findSpecificationProvider.Create(keys);
            _rootEntityProcessor.Recalculate(specification);
            foreach (var processor in _childEntityProcessors)
            {
                processor.Recalculate(keys);
            }
        }

        public void RecalculatePartially(IReadOnlyCollection<RecalculateAggregatePart> commands, Type partType)
        {
            var keys = commands.Select(x => x.AggregateInstanceId).ToArray();
            var processor = _childEntityProcessors.Single(x => x.ChildEntityType == partType);
            processor.RecalculatePartially(keys, commands);
        }

        public void Destroy(IReadOnlyCollection<DestroyAggregate> commands)
        {
            var keys = commands.Select(x => x.EntityId).ToArray();
            var specification = _findSpecificationProvider.Create(keys);
            _rootEntityProcessor.Destroy(specification);
            foreach (var processor in _childEntityProcessors)
            {
                processor.Destroy(keys);
            }
        }
    }
}