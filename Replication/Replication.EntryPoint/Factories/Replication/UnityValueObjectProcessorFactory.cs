using Microsoft.Practices.Unity;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Aggregates;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Equality;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    public class UnityValueObjectProcessorFactory<TEntity> : IValueObjectProcessorFactory<TEntity>
    {
        private readonly IUnityContainer _unityContainer;

        public UnityValueObjectProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IValueObjectProcessor<TEntity> Create(IValueObjectMetadata metadata)
        {
            var processorType = typeof(ValueObjectProcessor<,>).MakeGenericType(typeof(TEntity), metadata.ValueObjectType);

            var processor = _unityContainer.Resolve(
                processorType,
                ResolveDataChangesDetectorDependency(metadata),
                ResolveValueObjectFindSpecificationProvider(metadata));
            return (IValueObjectProcessor<TEntity>)processor;
        }

        private DependencyOverride ResolveDataChangesDetectorDependency(IValueObjectMetadata metadata)
        {
            var factory = (IValueObjectDataChangesDetectorFactory)_unityContainer.Resolve(
                typeof(ValueObjectDataChangesDetectorFactory<,>).MakeGenericType(metadata.GetType().GetGenericArguments()),
                new DependencyOverride(metadata.GetType(), metadata));
            var detector = factory.Create();
            return new DependencyOverride(detector.GetType(), detector);
        }

        private DependencyOverride ResolveValueObjectFindSpecificationProvider(IValueObjectMetadata metadata)
        {
            var metadatOverride = new DependencyOverride(metadata.GetType(), metadata);

            return new DependencyOverride(
                typeof(IFindSpecificationProvider<,>).MakeGenericType(metadata.ValueObjectType, typeof(AggregateOperation)),
                _unityContainer.Resolve(typeof(ValueObjectFindSpecificationProvider<,>).MakeGenericType(metadata.ValueObjectType), metadatOverride));
        }

        interface IValueObjectDataChangesDetectorFactory
        {
            object Create();
        }

        class ValueObjectDataChangesDetectorFactory<T, TKey> : IValueObjectDataChangesDetectorFactory
        {
            private readonly IQuery _query;
            private readonly ValueObjectMetadata<T, TKey> _metadata;
            private readonly IEqualityComparerFactory _equalityComparerFactory;

            public ValueObjectDataChangesDetectorFactory(ValueObjectMetadata<T, TKey> metadata, IEqualityComparerFactory equalityComparerFactory, IQuery query)
            {
                _metadata = metadata;
                _equalityComparerFactory = equalityComparerFactory;
                _query = query;
            }

            public object Create()
            {
                return new DataChangesDetector<T>(
                    _metadata.MapSpecificationProviderForSource,
                    _metadata.MapSpecificationProviderForTarget,
                    _equalityComparerFactory.CreateCompleteComparer<T>(),
                    _query);
            }
        }
    }
}