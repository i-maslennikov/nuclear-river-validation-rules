using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.Facts;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    public class UnityFactProcessorFactory : IFactProcessorFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityFactProcessorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IFactProcessor Create(IMetadataElement factMetadata)
        {
            var factType = factMetadata.GetType().GenericTypeArguments[0];
            var processorType = typeof(FactProcessor<>).MakeGenericType(factType);
            var processor = _unityContainer.Resolve(processorType,
                new DependencyOverride(factMetadata.GetType(), factMetadata),
                new DependencyOverride(typeof(IIdentityProvider<long>), DefaultIdentityProvider.Instance));
            return (IFactProcessor)processor;
        }
    }
}