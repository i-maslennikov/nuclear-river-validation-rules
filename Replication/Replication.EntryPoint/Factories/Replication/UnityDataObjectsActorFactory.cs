using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Model.Bit;
using NuClear.Replication.Core.API;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    public sealed class UnityDataObjectsActorFactory : IDataObjectsActorFactory
    {
        private static readonly HashSet<Type> DataObjectsToReplace =
            new HashSet<Type> { typeof(FirmCategoryForecast), typeof(FirmForecast), typeof(FirmCategoryStatistics), typeof(ProjectCategoryStatistics) };

        private readonly IUnityContainer _unityContainer;
        private readonly IDataObjectTypesProvider _dataObjectTypesProvider;

        public UnityDataObjectsActorFactory(IUnityContainer unityContainer, IDataObjectTypesProvider dataObjectTypesProvider)
        {
            _unityContainer = unityContainer;
            _dataObjectTypesProvider = dataObjectTypesProvider;
        }

        public IReadOnlyCollection<IActor> Create()
        {
            var actors = new List<IActor>();

            var dataObjectTypes = _dataObjectTypesProvider.Get<SyncDataObjectCommand>();
            foreach (var dataObjectType in dataObjectTypes)
            {
                var actor = (IActor)_unityContainer.Resolve(typeof(SyncDataObjectsActor<>).MakeGenericType(dataObjectType));
                actors.Add(actor);
            }

            dataObjectTypes = _dataObjectTypesProvider.Get<ReplaceFirmCategoryForecastCommand>()
                                                      .Concat(_dataObjectTypesProvider.Get<ReplaceFirmForecastCommand>())
                                                      .Concat(_dataObjectTypesProvider.Get<ReplaceFirmPopularityCommand>())
                                                      .Concat(_dataObjectTypesProvider.Get<ReplaceRubricPopularityCommand>())
                                                      .ToArray();
            foreach (var dataObjectType in dataObjectTypes)
            {
                var actor = (IActor)_unityContainer.Resolve(typeof(ReplaceMemoryBasedDataObjectsActor<>).MakeGenericType(dataObjectType));
                actors.Add(actor);
            }

            return actors;
        }
    }
}