using Microsoft.Practices.Unity;

using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.Facts;
using NuClear.River.Common.Metadata.Features;
using NuClear.River.Common.Metadata.Model;

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
            var identityProviderDependency = new DependencyOverride(typeof(IIdentityProvider<long>), DefaultIdentityProvider.Instance);
            var processor = _unityContainer.Resolve(processorType, metadataDependency, identityProviderDependency);
            return (IFactDependencyProcessor)processor;
        }
    }
}