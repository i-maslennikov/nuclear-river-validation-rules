using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider;
using NuClear.Replication.Core.API.Facts;
using NuClear.Replication.Core.Facts;
using NuClear.River.Common.Metadata.Identities;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    public class UnityStatisticsImporterFactory : IStatisticsImporterFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IMetadataProvider _metadataProvider;

        public UnityStatisticsImporterFactory(IUnityContainer unityContainer, IMetadataProvider metadataProvider)
        {
            _unityContainer = unityContainer;
            _metadataProvider = metadataProvider;
        }

        public IReadOnlyCollection<IStatisticsImporter> Create(Type dtoType)
        {
            MetadataSet metadataSet;
            if (!_metadataProvider.TryGetMetadata<ImportStatisticsMetadataIdentity>(out metadataSet))
            {
                throw new NotSupportedException($"Metadata for identity '{typeof(ImportStatisticsMetadataIdentity).Name}' cannot be found");
            }

            var dtoMetadataUri = GetMetadataUri(dtoType);
            return metadataSet.Metadata
                              .Where(x => dtoMetadataUri.IsBaseOf(x.Key))
                              .Select(x => Create(x.Value, dtoType))
                              .ToArray();
        }

        private IStatisticsImporter Create(IMetadataElement importStatisticsMetadata, Type dtoType)
        {
            var statisticsType = importStatisticsMetadata.GetType().GenericTypeArguments[0];
            var importerType = typeof(StatisticsFactImporter<,>).MakeGenericType(statisticsType, dtoType);
            var processor = _unityContainer.Resolve(importerType, new DependencyOverride(importStatisticsMetadata.GetType(), importStatisticsMetadata));
            return (IStatisticsImporter)processor;
        }

        private Uri GetMetadataUri(Type statisticsDtoType)
            => new Uri(ImportStatisticsMetadataIdentity.Instance.Id, statisticsDtoType.Name + "/");
    }
}