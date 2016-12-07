using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.Core.DataObjects;

namespace NuClear.ValidationRules.Replication.Host.Factories.Replication
{
    public sealed class UnityDataObjectsActorFactory : IDataObjectsActorFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IDataObjectTypesProvider _dataObjectTypesProvider;

        public UnityDataObjectsActorFactory(IUnityContainer unityContainer, IDataObjectTypesProvider dataObjectTypesProvider)
        {
            _unityContainer = unityContainer;
            _dataObjectTypesProvider = dataObjectTypesProvider;
        }

        public IReadOnlyCollection<IActor> Create()
        {
            var dataObjectTypes = _dataObjectTypesProvider.Get<ISyncDataObjectCommand>();
            if (dataObjectTypes.Any())
            {
                return dataObjectTypes.Select(dataObjectType => (IActor)_unityContainer.Resolve(typeof(SyncDataObjectsActor<>).MakeGenericType(dataObjectType)))
                                      .ToArray();
            }

            dataObjectTypes = _dataObjectTypesProvider.Get<IReplaceDataObjectCommand>();
            if (dataObjectTypes.Any())
            {
                return dataObjectTypes.Select(dataObjectType => (IActor)_unityContainer.Resolve(typeof(ReplaceDataObjectsActor<>).MakeGenericType(dataObjectType)))
                                      .ToArray();
            }

            return Array.Empty<IActor>();
        }
    }
}