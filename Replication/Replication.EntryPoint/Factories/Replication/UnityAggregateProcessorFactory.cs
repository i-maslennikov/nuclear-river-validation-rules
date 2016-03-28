using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Aggregates;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Equality;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    public sealed class UnityEntityProcessorFactory<TEntityType>
    {
        private readonly IUnityContainer _unityContainer;

        public UnityEntityProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public object Create(IMetadataElement metadata)
        {
            return new EntityProcessor<TEntityType>(
                _unityContainer.Resolve<IBulkRepository<TEntityType>>(),
                ResolveDataChangesDetectorDependency(metadata),
                ResolveValueObjectProcessorsDepencency(metadata));
        }

        private IReadOnlyCollection<IValueObjectProcessor<TEntityType>> ResolveValueObjectProcessorsDepencency(IMetadataElement metadata)
        {
            var valueObjectProcessorFactory = _unityContainer.Resolve<IValueObjectProcessorFactory<TEntityType>>();
            return metadata.Elements.OfType<IValueObjectMetadata>().Select(valueObjectProcessorFactory.Create).ToArray();
        }

        private DataChangesDetector<TEntityType> ResolveDataChangesDetectorDependency(IMetadataElement metadata)
        {
            var factory = (IEntityDataChangesDetectorFactory<TEntityType>)_unityContainer.Resolve(
                typeof(EntityDataChangesDetectorFactory<,>).MakeGenericType(metadata.GetType().GetGenericArguments()),
                new DependencyOverride(metadata.GetType(), metadata));
            return factory.Create();
        }

        interface IEntityDataChangesDetectorFactory<T>
        {
            DataChangesDetector<T> Create();
        }

        class EntityDataChangesDetectorFactory<T, TKey> : IEntityDataChangesDetectorFactory<T>
            where T : class, IIdentifiable<TKey>
        {
            private readonly IQuery _query;
            private readonly AggregateMetadata<T, TKey> _metadata;
            private readonly IEqualityComparerFactory _equalityComparerFactory;

            public EntityDataChangesDetectorFactory(AggregateMetadata<T, TKey> metadata, IEqualityComparerFactory equalityComparerFactory, IQuery query)
            {
                _metadata = metadata;
                _equalityComparerFactory = equalityComparerFactory;
                _query = query;
            }

            public DataChangesDetector<T> Create()
            {
                return new DataChangesDetector<T>(
                    _metadata.MapSpecificationProviderForSource,
                    _metadata.MapSpecificationProviderForTarget,
                    _equalityComparerFactory.CreateIdentityComparer<T>(),
                    _query);
            }
        }
    }

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
                ResolveAggregateFindSpecificationProvider(metadata));
            return (IAggregateProcessor)processor;
        }

        private DependencyOverride ResolveAggregateFindSpecificationProvider(IMetadataElement metadata)
        {
            var aggregateType = metadata.GetType().GenericTypeArguments[0];
            var aggregateKeyType = metadata.GetType().GenericTypeArguments[1];
            var metadataOverride = new DependencyOverride(metadata.GetType(), metadata);

            if (aggregateKeyType != typeof(long))
            {
                throw new NotImplementedException("Требуется реализовать IFindSpecificationProvider, способный извлекать из параметров команды идентификатор, отличный от long");
            }

            return new DependencyOverride(
                typeof(IFindSpecificationProvider<,>).MakeGenericType(aggregateType, typeof(AggregateOperation)),
                _unityContainer.Resolve(typeof(AggregateFindSpecificationProvider<>).MakeGenericType(aggregateType), metadataOverride));
        }
    }
}
