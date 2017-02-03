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

                    { typeof(AdvertisementAggregates::Advertisement), typeof(AdvertisementActors::AdvertisementAggregateRootActor) },
                    { typeof(AdvertisementAggregates::AdvertisementElementTemplate), typeof(AdvertisementActors::AdvertisementElementTemplateAggregateRootActor) },
                    { typeof(AdvertisementAggregates::Firm), typeof(AdvertisementActors::FirmAggregateRootActor) },
                    { typeof(AdvertisementAggregates::Order), typeof(AdvertisementActors::OrderAggregateRootActor) },
                    { typeof(AdvertisementAggregates::Position), typeof(AdvertisementActors::PositionAggregateRootActor) },

                    { typeof(ConsistencyAggregates::Order), typeof(ConsistencyActors::OrderAggregateRootActor) },

                    { typeof(FirmAggregates::Firm), typeof(FirmActors::FirmAggregateRootActor) },
                    { typeof(FirmAggregates::Order), typeof(FirmActors::OrderAggregateRootActor) },

                    { typeof(PriceAggregates::Category), typeof(PriceActors::CategoryAggregateRootActor) },
                    { typeof(PriceAggregates::Order), typeof(PriceActors::OrderAggregateRootActor) },
                    { typeof(PriceAggregates::Period), typeof(PriceActors::PeriodAggregateRootActor) },
                    { typeof(PriceAggregates::Position), typeof(PriceActors::PositionAggregateRootActor) },
                    { typeof(PriceAggregates::Price), typeof(PriceActors::PriceAggregateRootActor) },
                    { typeof(PriceAggregates::Project), typeof(PriceActors::ProjectAggregateRootActor) },
                    { typeof(PriceAggregates::Theme), typeof(PriceActors::ThemeAggregateRootActor) },

                    { typeof(ProjectAggregates::Category), typeof(ProjectActors::CategoryAggregateRootActor) },
                    { typeof(ProjectAggregates::FirmAddress), typeof(ProjectActors::FirmAddressAggregateRootActor) },
                    { typeof(ProjectAggregates::Order), typeof(ProjectActors::OrderAggregateRootActor) },
                    { typeof(ProjectAggregates::Position), typeof(ProjectActors::PositionAggregateRootActor) },
                    { typeof(ProjectAggregates::Project), typeof(ProjectActors::ProjectAggregateRootActor) },

                    { typeof(ThemeAggregates::Category), typeof(ThemeActors::CategoryAggregateRootActor) },
                    { typeof(ThemeAggregates::Order), typeof(ThemeActors::OrderAggregateRootActor) },
                    { typeof(ThemeAggregates::Project), typeof(ThemeActors::ProjectAggregateRootActor) },
                    { typeof(ThemeAggregates::Theme), typeof(ThemeActors::ThemeAggregateRootActor) },
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