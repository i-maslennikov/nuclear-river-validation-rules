using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;

namespace NuClear.ValidationRules.Replication.Host.Factories.Replication
{
    public sealed class UnityDataObjectsActorFactory : IDataObjectsActorFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityDataObjectsActorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IReadOnlyCollection<IActor> Create(IReadOnlyCollection<Type> dataObjectTypes)
        {
            var actors = dataObjectTypes
                .SelectMany(GetActorType)
                .Select(x => (IActor)_unityContainer.Resolve(x))
                .ToList();

            return actors;
        }

        private IEnumerable<Type> GetActorType(Type dataObjectType)
        {
            if (IsRegistered(typeof(IStorageBasedDataObjectAccessor<>), dataObjectType))
            {
                yield return typeof(SyncDataObjectsActor<>).MakeGenericType(dataObjectType);
            }

            if (IsRegistered(typeof(IStorageBasedEntityNameAccessor<>), dataObjectType))
            {
                yield return typeof(SyncEntityNameActor<>).MakeGenericType(dataObjectType);
            }

            if (IsRegistered(typeof(IMemoryBasedDataObjectAccessor<>), dataObjectType))
            {
                yield return typeof(ReplaceDataObjectsActor<>).MakeGenericType(dataObjectType);
            }
        }

        private bool IsRegistered(Type genericContract, Type dataObjectType)
        {
            var type = genericContract.MakeGenericType(dataObjectType);
            return _unityContainer.IsRegistered(type);
        }
    }
}