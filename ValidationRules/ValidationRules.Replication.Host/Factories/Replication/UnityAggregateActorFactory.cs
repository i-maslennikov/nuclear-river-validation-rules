using System;
using System.Collections.Generic;

using Microsoft.Practices.Unity;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.ValidationRules.Replication.PriceRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Replication.Host.Factories.Replication
{
    public sealed class UnityAggregateActorFactory : IAggregateActorFactory
    {
        private readonly IUnityContainer _unityContainer;

        private static readonly IReadOnlyDictionary<Type, Type> AggregateRootActors =
            new Dictionary<Type, Type>
                {
                    { typeof(Price), typeof(PriceAggregateRootActor) },
                    { typeof(Order), typeof(OrderAggregateRootActor) },
                    { typeof(Period), typeof(PeriodAggregateRootActor) },
                    { typeof(Position), typeof(PositionAggregateRootActor) },
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