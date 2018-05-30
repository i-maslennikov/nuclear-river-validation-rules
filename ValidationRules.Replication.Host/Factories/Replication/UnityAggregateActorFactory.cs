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

using AdvertisementAggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using AdvertisementActors = NuClear.ValidationRules.Replication.AdvertisementRules.Aggregates;
using ThemeAggregates = NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;
using ThemeActors = NuClear.ValidationRules.Replication.ThemeRules.Aggregates;
using SystemAggregates = NuClear.ValidationRules.Storage.Model.SystemRules.Aggregates;
using SystemActors = NuClear.ValidationRules.Replication.SystemRules.Aggregates;

using ProjectAggregates = NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;
using ProjectActors = NuClear.ValidationRules.Replication.ProjectRules.Aggregates;

namespace NuClear.ValidationRules.Replication.Host.Factories.Replication
{
    public sealed class UnityAggregateActorFactory : IAggregateActorFactory
    {
        private readonly IUnityContainer _unityContainer;

        private static readonly IReadOnlyDictionary<Type, Type> AggregateRootActors =
            new Dictionary<Type, Type>
                {
                    { typeof(AccountAggregates::Order), typeof(AccountActors::OrderAggregateRootActor) },
                    { typeof(AccountAggregates::Account), typeof(AccountActors::AccountAggregateRootActor) },

                    { typeof(AdvertisementAggregates::Order), typeof(AdvertisementActors::OrderAggregateRootActor) },

                    { typeof(ConsistencyAggregates::Order), typeof(ConsistencyActors::OrderAggregateRootActor) },

                    { typeof(FirmAggregates::Firm), typeof(FirmActors::FirmAggregateRootActor) },
                    { typeof(FirmAggregates::Order), typeof(FirmActors::OrderAggregateRootActor) },

                    { typeof(PriceAggregates::Firm), typeof(PriceActors::FirmAggregateRootActor) },
                    { typeof(PriceAggregates::Order), typeof(PriceActors::OrderAggregateRootActor) },
                    { typeof(PriceAggregates::Period), typeof(PriceActors::PeriodAggregateRootActor) },
                    { typeof(PriceAggregates::Price), typeof(PriceActors::PriceAggregateRootActor) },
                    { typeof(PriceAggregates::Ruleset), typeof(PriceActors::RulesetAggregateRootActor) },

                    { typeof(ProjectAggregates::Order), typeof(ProjectActors::OrderAggregateRootActor) },
                    { typeof(ProjectAggregates::Project), typeof(ProjectActors::ProjectAggregateRootActor) },

                    { typeof(ThemeAggregates::Order), typeof(ThemeActors::OrderAggregateRootActor) },
                    { typeof(ThemeAggregates::Project), typeof(ThemeActors::ProjectAggregateRootActor) },
                    { typeof(ThemeAggregates::Theme), typeof(ThemeActors::ThemeAggregateRootActor) },

                    { typeof(SystemAggregates::SystemStatus), typeof(SystemActors::SystemStatusAggregateRootActor) },
                };

        public UnityAggregateActorFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public IReadOnlyCollection<IActor> Create(IReadOnlyCollection<Type> aggregateRootTypes)
            => aggregateRootTypes.Select(CreateActorForAggregateRoot).ToList();

        private AggregateActor CreateActorForAggregateRoot(Type aggregateRootType)
        {
            if (!AggregateRootActors.TryGetValue(aggregateRootType, out var aggregateRootActorType))
            {
                throw new ArgumentException($"Can't find aggregate actor for type {aggregateRootType.GetFriendlyName()}");
            }

            var aggregateRootActor = (IAggregateRootActor)_unityContainer.Resolve(aggregateRootActorType);
            return _unityContainer.Resolve<AggregateActor>(new DependencyOverride<IAggregateRootActor>(aggregateRootActor));
        }
    }
}