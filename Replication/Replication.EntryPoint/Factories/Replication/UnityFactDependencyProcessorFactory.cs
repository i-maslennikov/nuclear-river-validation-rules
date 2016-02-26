using Microsoft.Practices.Unity;

using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.Facts;
using NuClear.River.Common.Metadata.Features;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    internal class UnityFactDependencyProcessorFactory : IFactDependencyProcessorFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityFactDependencyProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IFactDependencyProcessor Create(IFactDependencyFeature metadata)
        {
            var processorType = typeof(FactDependencyProcessor<>).MakeGenericType(metadata.DependencyType);
            var metadataDependency = new DependencyOverride(typeof(IFactDependencyFeature<>).MakeGenericType(metadata.DependencyType), metadata);
            var processor = _unityContainer.Resolve(processorType, metadataDependency);
            return (IFactDependencyProcessor)processor;
        }
    }
}