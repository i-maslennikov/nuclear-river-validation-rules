using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Aggregates;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Equality;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Readings;

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
            var processorType = typeof(AggregateProcessor<,>).MakeGenericType(aggregateType, aggregateKeyType);

            var processor = _unityContainer.Resolve(
                processorType,
                ResolveDataChangesDetectorDependency(metadata),
                ResolveValueObjectProcessorsDepencency(metadata),
                ResolveAggregateFindSpecificationProvider(metadata));
            return (IAggregateProcessor)processor;
        }

        private DependencyOverride ResolveValueObjectProcessorsDepencency(IMetadataElement metadata)
        {
            var valueObjectProcessorFactory = _unityContainer.Resolve<IValueObjectProcessorFactory>();
            var processors = metadata.Elements.OfType<IValueObjectMetadata>().Select(valueObjectProcessorFactory.Create).ToArray();
            return new DependencyOverride(typeof(IReadOnlyCollection<IValueObjectProcessor>), processors);
        }

        private DependencyOverride ResolveDataChangesDetectorDependency(IMetadataElement metadata)
        {
            var factory = (IAggregateDataChangesDetectorFactory)_unityContainer.Resolve(
                typeof(AggregateDataChangesDetectorFactory<,>).MakeGenericType(metadata.GetType().GetGenericArguments()),
                new DependencyOverride(metadata.GetType(), metadata));
            var detector = factory.Create();
            return new DependencyOverride(detector.GetType(), detector);
        }

        private DependencyOverride ResolveAggregateFindSpecificationProvider(IMetadataElement metadata)
        {
            var aggregateType = metadata.GetType().GenericTypeArguments[0];
            var aggregateKeyType = metadata.GetType().GenericTypeArguments[1];
            var metadataOverride = new DependencyOverride(metadata.GetType(), metadata);

            return new DependencyOverride(
                typeof(IFindSpecificationProvider<>).MakeGenericType(aggregateType),
                _unityContainer.Resolve(typeof(AggregateFindSpecificationProvider<,>).MakeGenericType(aggregateType, aggregateKeyType), metadataOverride));
        }

        interface IAggregateDataChangesDetectorFactory
        {
            object Create();
        }

        class AggregateDataChangesDetectorFactory<T, TKey> : IAggregateDataChangesDetectorFactory
            where T : class, IIdentifiable<TKey>
        {
            private readonly IQuery _query;
            private readonly AggregateMetadata<T, TKey> _metadata;
            private readonly IEqualityComparerFactory _equalityComparerFactory;

            public AggregateDataChangesDetectorFactory(AggregateMetadata<T, TKey> metadata, IEqualityComparerFactory equalityComparerFactory, IQuery query)
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
                    _equalityComparerFactory.CreateIdentityComparer<T>(),
                    _query);
            }
        }
    }
}
