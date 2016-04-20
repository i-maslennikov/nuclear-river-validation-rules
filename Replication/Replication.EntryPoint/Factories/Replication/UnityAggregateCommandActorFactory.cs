using System;
using System.Collections.Generic;

using Microsoft.Practices.Unity;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Model.CI;
using NuClear.CustomerIntelligence.Domain.Model.Statistics;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;

namespace NuClear.Replication.EntryPoint.Factories.Replication
{
    public sealed class UnityAggregateCommandActorFactory : IAggregateCommandActorFactory
    {
        private readonly IUnityContainer _unityContainer;

        private static readonly IReadOnlyDictionary<Type, Type> AggregateActors =
            new Dictionary<Type, Type>
                {
                    { typeof(Firm), typeof(FirmAggregateActor) },
                    { typeof(ProjectStatistics), typeof(ProjectStatisticsAggregateActor) }
                };

        public UnityAggregateCommandActorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IActor Create(Type commandType, Type aggregateRootType)
        {
            var aggregateActorType = AggregateActors[aggregateRootType];
            var aggregateRootActor = (IAggregateRootActor)_unityContainer.Resolve(aggregateActorType);
            if (commandType == typeof(DestroyAggregateCommand))
            {
                return new LeafToRootActor(aggregateRootActor);
            }

            return new RootToLeafActor(aggregateRootActor);
        }
    }
}