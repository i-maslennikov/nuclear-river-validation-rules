using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.Facts;
using NuClear.River.Common.Metadata.Elements;
using NuClear.River.Common.Metadata.Identities;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    public class UnityImportDocumentMetadataProcessorFactory : IImportDocumentMetadataProcessorFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IMetadataProvider _metadataProvider;

        public UnityImportDocumentMetadataProcessorFactory(IUnityContainer unityContainer, IMetadataProvider metadataProvider)
        {
            _unityContainer = unityContainer;
            _metadataProvider = metadataProvider;
        }

        public IImportDocumentMetadataProcessor Create(Type dtoType)
        {
            IMetadataElement metadata;
            if (!_metadataProvider.TryGetMetadata(GetMetadataUri(dtoType), out metadata))
            {
                throw new NotSupportedException($"Metadata for dto type '{dtoType.Name}' cannot be found");
            }

            var featureProcessors = new List<object>();
            foreach (var feature in metadata.Features.Where(IsImportDocumentFeature))
            {
                var featureProcessorType = typeof(ImportDocumentFeatureProcessor<,>).MakeGenericType(feature.GetType().GenericTypeArguments);
                var featureProcessor = _unityContainer.Resolve(featureProcessorType, new DependencyOverride(feature.GetType(), feature));
                featureProcessors.Add(featureProcessor);
            }
            var featureProcessorsArray = Array.CreateInstance(typeof(IImportDocumentFeatureProcessor<>).MakeGenericType(dtoType), featureProcessors.Count);
            Array.Copy(featureProcessors.ToArray(), featureProcessorsArray, featureProcessors.Count);

            var processorType = typeof(ImportDocumentMetadataProcessor<>).MakeGenericType(dtoType);
            var processor = _unityContainer.Resolve(processorType,
                                                    new DependencyOverride(metadata.GetType(), metadata),
                                                    new DependencyOverride(MakeProcessorsCollectionType(dtoType), featureProcessorsArray));
            return (IImportDocumentMetadataProcessor)processor;
        }

        private Uri GetMetadataUri(Type dtoType)
            => ImportDocumentMetadataIdentity.Instance.Id.WithRelative(new Uri(dtoType.Name, UriKind.Relative));

        private Type MakeProcessorsCollectionType(Type dtoType)
            => typeof(IReadOnlyCollection<>).MakeGenericType(typeof(IImportDocumentFeatureProcessor<>).MakeGenericType(dtoType));

        private bool IsImportDocumentFeature(object feature)
            => feature.GetType().IsGenericType && feature.GetType().GetGenericTypeDefinition() == typeof(ImportDocumentFeature<,>);
    }
}