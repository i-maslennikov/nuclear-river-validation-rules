using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Core;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.Facts;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Equality;
using NuClear.River.Common.Metadata.Features;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Readings;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    public class UnityFactProcessorFactory : IFactProcessorFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IFactDependencyProcessorFactory _dependencyProcessorFactory;

        public UnityFactProcessorFactory(IUnityContainer unityContainer, IFactDependencyProcessorFactory dependencyProcessorFactory)
        {
            _unityContainer = unityContainer;
            _dependencyProcessorFactory = dependencyProcessorFactory;
        }

        public IFactProcessor Create(IMetadataElement factMetadata)
        {
            var factType = factMetadata.GetType().GenericTypeArguments[0];
            var processorType = typeof(FactProcessor<>).MakeGenericType(factType);
            var processor = _unityContainer.Resolve(processorType,
                ResolveDataChangesDetectorDependency(factMetadata),
                ResolveDepencencyProcessorsDependency(factMetadata));
            return (IFactProcessor)processor;
        }

        private DependencyOverride ResolveDepencencyProcessorsDependency(IMetadataElement metadata)
        {
            return new DependencyOverride(
                typeof(IReadOnlyCollection<IFactDependencyProcessor>),
                metadata.Features.OfType<IFactDependencyFeature>().Select(_dependencyProcessorFactory.Create).ToArray());
        }

        private DependencyOverride ResolveDataChangesDetectorDependency(IMetadataElement metadata)
        {
            var factory = (IFactDataChangesDetectorFactory)_unityContainer.Resolve(
                typeof(FactDataChangesDetectorFactory<>).MakeGenericType(metadata.GetType().GetGenericArguments()),
                new DependencyOverride(metadata.GetType(), metadata));
            var detector = factory.Create();
            return new DependencyOverride(detector.GetType(), detector);
        }

        interface IFactDataChangesDetectorFactory
        {
            object Create();
        }

        class FactDataChangesDetectorFactory<T> : IFactDataChangesDetectorFactory
            where T : class, IIdentifiable<long>
        {
            private readonly IQuery _query;
            private readonly FactMetadata<T> _metadata;
            private readonly IEqualityComparerFactory _equalityComparerFactory;

            public FactDataChangesDetectorFactory(FactMetadata<T> metadata, IEqualityComparerFactory equalityComparerFactory, IQuery query)
            {
                _metadata = metadata;
                _equalityComparerFactory = equalityComparerFactory;
                _query = query;
            }

            public object Create()
            {
                return new FactChangesDetector<T>(
                    _metadata.MapSpecificationProviderForSource,
                    _metadata.MapSpecificationProviderForTarget,
                    _equalityComparerFactory.CreateIdentityComparer<T>(),
                    _equalityComparerFactory.CreateCompleteComparer<T>(),
                    _query);
            }
        }
    }
}