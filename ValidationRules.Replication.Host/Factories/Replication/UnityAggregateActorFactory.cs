using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;

using AccountAggregates = NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using AccountActors = NuClear.ValidationRules.Replication.AccountRules.Aggregates;
using PriceAggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using PriceActors = NuClear.ValidationRules.Replication.PriceRules.Aggregates;
using ConsistencyAggregates = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using ConsistencyActors = NuClear.ValidationRules.Replication.ConsistencyRules.Aggregates;
using FirmAggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using FirmActors = NuClear.ValidationRules.Replication.FirmRules.Aggregates;

namespace NuClear.ValidationRules.Replication.Host.Factories.Replication
{
    public sealed class UnityAggregateActorFactory : IAggregateActorFactory
    {
        private readonly IUnityContainer _unityContainer;

        private static readonly IReadOnlyDictionary<Type, Type> AggregateRootActors =
            new Dictionary<Type, Type>
                {
                    { typeof(PriceAggregates::Price), typeof(PriceActors::PriceAggregateRootActor) },
                    { typeof(PriceAggregates::Project), typeof(PriceActors::ProjectAggregateRootActor) },
                    { typeof(PriceAggregates::Order), typeof(PriceActors::OrderAggregateRootActor) },
                    { typeof(PriceAggregates::Period), typeof(PriceActors::PeriodAggregateRootActor) },
                    { typeof(PriceAggregates::Position), typeof(PriceActors::PositionAggregateRootActor) },

                    { typeof(AccountAggregates::Order), typeof(AccountActors::OrderAggregateRootActor) },
                    { typeof(AccountAggregates::Account), typeof(AccountActors::AccountAggregateRootActor) },

                    { typeof(ConsistencyAggregates::Order), typeof(ConsistencyActors::OrderAggregateRootActor) },

                    { typeof(FirmAggregates::Order), typeof(FirmActors::OrderAggregateRootActor) },
                    { typeof(FirmAggregates::Firm), typeof(FirmActors::FirmAggregateRootActor) },
                };

        public UnityAggregateActorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IReadOnlyCollection<IActor> Create(IReadOnlyCollection<Type> aggregateRootTypes)
        {
            var actors = new List<IActor>();
            foreach (var aggregateRootType in AggregateRootActors.Keys)
            {
                if (aggregateRootTypes.Contains(aggregateRootType))
                {
                    var aggregateRootActorType = AggregateRootActors[aggregateRootType];
                    var aggregateRootActor = (IAggregateRootActor)_unityContainer.Resolve(aggregateRootActorType);
                    actors.Add(new AggregateActor(aggregateRootActor));
                }
            }

            return actors;
        }
    }
}