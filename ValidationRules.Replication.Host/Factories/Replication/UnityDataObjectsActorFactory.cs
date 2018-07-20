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
        private readonly Type _syncDataObjectsActorType = typeof(SyncDataObjectsActor<>);
        // ReSharper disable once RedundantNameQualifier
        private readonly Type _replaceDataObjectsActorType = typeof(ValidationRules.Replication.ReplaceDataObjectsActor<>);

        private readonly IUnityContainer _unityContainer;
        private readonly IDataObjectTypesProvider _dataObjectTypesProvider;

        public UnityDataObjectsActorFactory(IUnityContainer unityContainer, IDataObjectTypesProvider dataObjectTypesProvider)
        {
            _unityContainer = unityContainer;
            _dataObjectTypesProvider = dataObjectTypesProvider;
        }

        public IReadOnlyCollection<IActor> Create()
        {
            var syncActorTypes = _dataObjectTypesProvider.Get<ISyncDataObjectCommand>()
                                                         .Select(x => _syncDataObjectsActorType.MakeGenericType(x));
            var replaceActorTypes = _dataObjectTypesProvider.Get<IReplaceDataObjectCommand>()
                                                            .Select(x => _replaceDataObjectsActorType.MakeGenericType(x));

            var actors = syncActorTypes.Concat(replaceActorTypes)
                                       .Select(x => (IActor)_unityContainer.Resolve(x))
                                       .ToList();

            return actors;
        }
    }
}