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
using NuClear.ValidationRules.Domain.Model;

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
            if (am.EntityKeyType == typeof(long))
            {
                var factory = (IAggregateProcessorFactory)_unityContainer.Resolve(typeof(UnityAggregateProcessorFactory<,>).MakeGenericType(am.EntityType, am.EntityKeyType));
                return factory.Create(am);
            }

            if (am.EntityKeyType == typeof(PeriodKey))
            {
                var factory = (IAggregateProcessorFactory)_unityContainer.Resolve(typeof(UnityAggregateProcessorFactory<,>).MakeGenericType(am.EntityType, am.EntityKeyType));
                return factory.Create(am);
            }


            throw new ArgumentException($"требуются доработки для поддержки ключа {am.EntityKeyType.Name}");
        }
    }

    internal sealed class UnityAggregateProcessorFactory<TEntity, TEntityKey> : IAggregateProcessorFactory
        where TEntity : class, IIdentifiable<TEntityKey>
    {
        private readonly IUnityContainer _unityContainer;

        public UnityAggregateProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IAggregateProcessor Create(IMetadataElement metadata)
        {
            var am = (IAggregateMetadata<TEntity>)metadata;

            var processor = new AggregateProcessor<TEntity, TEntityKey>(
                _unityContainer.Resolve<FindSpecificationProvider<TEntity, TEntityKey>>(),
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

        private IReadOnlyCollection<IChildEntityProcessor<TEntityKey>> ResolveChildProcessors(IAggregateMetadata<TEntity> metadata)
        {
            var processors = new List<IChildEntityProcessor<TEntityKey>>();
            foreach (var feature in metadata.Features.OfType<IChildEntityFeature>())
            {
                processors.Add(CreateChildEntityProcessor(metadata, feature));
            }

            return processors;
        }

        private IChildEntityProcessor<TEntityKey> CreateChildEntityProcessor(IAggregateMetadata<TEntity> metadata, IChildEntityFeature feature)
        {
            var childMetadata = metadata.Elements.OfType<IAggregateMetadata>().Single(x => x.EntityType == feature.ChildEntityType);
            var factoryType = typeof(UnityChildEntityProcessorFactory<,,>).MakeGenericType(metadata.EntityKeyType, feature.ChildEntityType, feature.ChildEntityKeyType);
            var factory = (IChildEntityProcessorFactory<TEntityKey>)_unityContainer.Resolve(factoryType);
            var processor = factory.Create(metadata, childMetadata, feature);
            return processor;
        }
    }
}
