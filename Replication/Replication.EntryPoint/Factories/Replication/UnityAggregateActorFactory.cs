using System;
using System.Collections.Generic;

using Microsoft.Practices.Unity;

using NuClear.CustomerIntelligence.Domain.Actors;
using NuClear.CustomerIntelligence.Storage.Model.CI;
using NuClear.CustomerIntelligence.Storage.Model.Statistics;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Actors;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    public sealed class UnityAggregateActorFactory : IAggregateActorFactory
    {
        private readonly IUnityContainer _unityContainer;

        private static readonly IReadOnlyDictionary<Type, Type> AggregateRootActors =
            new Dictionary<Type, Type>
                {
                    { typeof(Firm), typeof(FirmAggregateRootActor) },
                    { typeof(Client), typeof(ClientAggregateRootActor) },
                    { typeof(Territory), typeof(TerritoryAggregateRootActor) },
                    { typeof(CategoryGroup), typeof(CategoryGroupAggregateRootActor) },
                    { typeof(Project), typeof(ProjectAggregateRootActor) },
                    { typeof(ProjectStatistics), typeof(ProjectStatisticsAggregateRootActor) }
                };

        public UnityAggregateActorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IActor Create(Type aggregateRootType)
        {
            var aggregateRootActorType = AggregateRootActors[aggregateRootType];
            var aggregateRootActor = (IAggregateRootActor)_unityContainer.Resolve(aggregateRootActorType);
            return new AggregateActor(aggregateRootActor);
        }
    }
}