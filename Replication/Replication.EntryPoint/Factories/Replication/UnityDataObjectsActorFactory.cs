using System;
using System.Collections.Generic;

using Microsoft.Practices.Unity;

using NuClear.CustomerIntelligence.Domain.Model.Bit;
using NuClear.Replication.Core.API;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    public class UnityDataObjectsActorFactory : IDataObjectsActorFactory
    {
        private static readonly HashSet<Type> DataObjectsToReplace =
            new HashSet<Type> { typeof(FirmCategoryForecast), typeof(FirmForecast), typeof(FirmCategoryStatistics), typeof(ProjectCategoryStatistics) };

        private readonly IUnityContainer _unityContainer;

        public UnityDataObjectsActorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IActor Create(Type dataObjectType)
        {
            var actorType = DataObjectsToReplace.Contains(dataObjectType)
                                ? typeof(ReplaceMemoryBasedDataObjectsActor<>).MakeGenericType(dataObjectType)
                                : typeof(SyncDataObjectsActor<>).MakeGenericType(dataObjectType);

            return (IActor)_unityContainer.Resolve(actorType);
        }
    }
}