using System;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Aggregates;
using NuClear.Replication.Core.API.Aggregates;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    public sealed class UnityAggregateProcessorFactory : IAggregateProcessorFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityAggregateProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IAggregateProcessor Create(IMetadataElement metadata)
        {
            var aggregateType = metadata.GetType().GenericTypeArguments[0];
            var aggregateKeyType = metadata.GetType().GenericTypeArguments[1];
            if (aggregateKeyType != typeof(long))
            {
                throw new ArgumentException($"aggregate key type {aggregateKeyType.Name} is not supported");
            }

            var processorType = typeof(AggregateProcessor<>).MakeGenericType(aggregateType);

            var processor = _unityContainer.Resolve(
                processorType,
                ResolveAggregateFindSpecificationProvider(aggregateType, aggregateKeyType),
                ResolveRootEntityProcessor(aggregateType, metadata));

            return (IAggregateProcessor)processor;
        }

        private ResolverOverride ResolveRootEntityProcessor(Type rootEntityType, IMetadataElement metadata)
        {
            var processorFactoryType = typeof(UnityEntityProcessorFactory<>).MakeGenericType(rootEntityType);
            var processorFactory = (IEntityProcessorFactory)_unityContainer.Resolve(processorFactoryType);
            var processor = processorFactory.Create(metadata);
            return new DependencyOverride(processor.GetType(), processor);
        }

        private DependencyOverride ResolveAggregateFindSpecificationProvider(Type aggregateType, Type aggregateKeyType)
        {
            return new DependencyOverride(
                typeof(IFindSpecificationProvider<,>).MakeGenericType(aggregateType, aggregateKeyType),
                _unityContainer.Resolve(typeof(FindSpecificationProvider<,>).MakeGenericType(aggregateType, aggregateKeyType)));
        }
    }
}
