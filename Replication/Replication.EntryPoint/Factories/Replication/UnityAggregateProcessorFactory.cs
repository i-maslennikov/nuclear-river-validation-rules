using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Aggregates;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Model;

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
            var am = (IAggregateMetadata)metadata;
            if (am.EntityKeyType != typeof(long))
            {
                throw new ArgumentException($"требуются доработки для поддержки ключа {am.EntityKeyType.Name}");
            }

            var factory = (IAggregateProcessorFactory)_unityContainer.Resolve(typeof(UnityAggregateProcessorFactory<>).MakeGenericType(am.EntityType));
            return factory.Create(am);
        }
    }

    internal sealed class UnityAggregateProcessorFactory<TEntity> : IAggregateProcessorFactory
        where TEntity : class, IIdentifiable<long>
    {
        private readonly IUnityContainer _unityContainer;

        public UnityAggregateProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IAggregateProcessor Create(IMetadataElement metadata)
        {
            var am = (IAggregateMetadata<TEntity>)metadata;

            var processor = new AggregateProcessor<TEntity>(
                _unityContainer.Resolve<FindSpecificationProvider<TEntity, long>>(),
                ResolveRootEntityProcessor(am),
                ResolveChildProcessors(am));

            return processor;
        }

        private EntityProcessor<TEntity> ResolveRootEntityProcessor(IAggregateMetadata<TEntity> metadata)
        {
            var processorFactory = _unityContainer.Resolve<UnityEntityProcessorFactory<TEntity>>();
            var processor = processorFactory.Create(metadata);
            return processor;
        }

        private IReadOnlyCollection<IChildEntityProcessor<long>> ResolveChildProcessors(IAggregateMetadata<TEntity> metadata)
        {
            var processors = new List<IChildEntityProcessor<long>>();
            foreach (var feature in metadata.Features.OfType<IChildEntityFeature>())
            {
                var childMetadata = metadata.Elements.OfType<IAggregateMetadata>().Single(x => x.EntityType == feature.ChildEntityType);
                var factoryType = typeof(UnityChildEntityProcessorFactory<,>).MakeGenericType(metadata.EntityKeyType, feature.ChildEntityType);
                var factory = (IChildEntityProcessorFactory<long>)_unityContainer.Resolve(factoryType);
                var processor = factory.Create(metadata, childMetadata, feature);
                processors.Add(processor);
            }

            return processors;
        }
    }
}
