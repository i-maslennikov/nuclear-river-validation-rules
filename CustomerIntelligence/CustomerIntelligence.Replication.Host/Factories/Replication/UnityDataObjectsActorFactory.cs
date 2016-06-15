using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.Core.DataObjects;

namespace NuClear.CustomerIntelligence.Replication.Host.Factories.Replication
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
            var syncActorTypes = _dataObjectTypesProvider.Get<ISyncDataObjectCommand>().Select(x => typeof(SyncDataObjectsActor<>).MakeGenericType(x));
            var replaceActorTypes = _dataObjectTypesProvider.Get<IReplaceDataObjectCommand>().Select(x => typeof(ReplaceDataObjectsActor<>).MakeGenericType(x));

            var actors = syncActorTypes
                        .Concat(replaceActorTypes)
                        .Select(x => (IActor)_unityContainer.Resolve(x)).ToList();

            return actors;
        }
    }
}