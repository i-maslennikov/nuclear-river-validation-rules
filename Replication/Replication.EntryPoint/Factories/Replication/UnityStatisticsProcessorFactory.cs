using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Aggregates;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Equality;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    public class UnityStatisticsProcessorFactory : IStatisticsProcessorFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityStatisticsProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IStatisticsProcessor Create(IMetadataElement metadata)
        {
            var statisticsType = metadata.GetType().GenericTypeArguments[0];
            var processorType = typeof(StatisticsProcessor<>).MakeGenericType(statisticsType);
            var processor = _unityContainer.Resolve(processorType,
                                                    new DependencyOverride(metadata.GetType(), metadata),
                                                    ResolveAggregateFindSpecificationProvider(metadata),
                                                    ResolveDataChangesDetectorDependency(metadata));
            return (IStatisticsProcessor)processor;
        }

        private DependencyOverride ResolveAggregateFindSpecificationProvider(IMetadataElement metadata)
        {
            var statisticsType = metadata.GetType().GenericTypeArguments[0];
            var metadataOverride = new DependencyOverride(metadata.GetType(), metadata);

            return new DependencyOverride(
                typeof(IFindSpecificationProvider<,>).MakeGenericType(statisticsType, typeof(RecalculateStatisticsOperation)),
                _unityContainer.Resolve(typeof(StatisticsFindSpecificationProvider<>).MakeGenericType(statisticsType), metadataOverride));
        }

        private DependencyOverride ResolveDataChangesDetectorDependency(IMetadataElement metadata)
        {
            var statisticsType = metadata.GetType().GenericTypeArguments[0];

            var factory = (IDataChangesDetectorFactory)_unityContainer.Resolve(
                typeof(DataChangesDetectorFactory<>).MakeGenericType(statisticsType),
                new DependencyOverride(metadata.GetType(), metadata));
            var detector = factory.Create();
            return new DependencyOverride(detector.GetType(), detector);
        }

        interface IDataChangesDetectorFactory
        {
            object Create();
        }

        class DataChangesDetectorFactory<T> : IDataChangesDetectorFactory
            where T : class
        {
            private readonly IQuery _query;
            private readonly StatisticsRecalculationMetadata<T, StatisticsKey> _metadata;
            private readonly IEqualityComparerFactory _equalityComparerFactory;

            public DataChangesDetectorFactory(StatisticsRecalculationMetadata<T, StatisticsKey> metadata, IEqualityComparerFactory equalityComparerFactory, IQuery query)
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