using System;
using System.Collections.Generic;
using System.Linq;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

using AccountAggregates = NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using AdvertisementAggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using PriceAggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using ProjectAggregates = NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;
using ConsistencyAggregates = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using FirmAggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using ThemeAggregates = NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;
using SystemAggregates = NuClear.ValidationRules.Storage.Model.SystemRules.Aggregates;

namespace NuClear.ValidationRules.OperationsProcessing
{
    internal static partial class EntityTypeMap
    {
        private static readonly Dictionary<Tuple<Type, Type>, IReadOnlyCollection<Type>> AggregateEventMapping =
            new Dictionary<Tuple<Type, Type>, IList<Type>>()
                // AccountAggregates
                .Aggregate<AccountAggregates::Account>(
                    x => x.Match<Facts::Account>()
                          .DependOn<Facts::Order>()
                          .DependOn<Facts::AccountDetail>()
                          .DependOn<Facts::OrderPosition>()
                          .DependOn<Facts::ReleaseWithdrawal>())
                .Aggregate<AccountAggregates::Order>(
                    x => x.Match<Facts::Order>()
                          .DependOn<Facts::UnlimitedOrder>()
                          .DependOn<Facts::Account>())

                // AdvertisementAggregates
                .Aggregate<AdvertisementAggregates::Order>(
                    x => x.Match<Facts::Order>()
                          .DependOn<Facts::Advertisement>()
                          .DependOn<Facts::OrderPosition>()
                          .DependOn<Facts::OrderPositionAdvertisement>()
                          .DependOn<Facts::Position>()
                          .DependOn<Facts::PositionChild>()
                          .DependOn<Facts::PricePosition>()
                          .DependOn<Facts::Project>())

                // ConsistencyAggregates
                .Aggregate<ConsistencyAggregates::Order>(
                    x => x.Match<Facts::Order>()
                          .DependOn<Facts::Bargain>()
                          .DependOn<Facts::BargainScanFile>()
                          .DependOn<Facts::Bill>()
                          .DependOn<Facts::BranchOffice>()
                          .DependOn<Facts::BranchOfficeOrganizationUnit>()
                          .DependOn<Facts::Category>()
                          .DependOn<Facts::Deal>()
                          .DependOn<Facts::FirmAddress>()
                          .DependOn<Facts::FirmAddressCategory>()
                          .DependOn<Facts::LegalPerson>()
                          .DependOn<Facts::LegalPersonProfile>()
                          .DependOn<Facts::OrderPosition>()
                          .DependOn<Facts::OrderPositionAdvertisement>()
                          .DependOn<Facts::OrderScanFile>()
                          .DependOn<Facts::Position>()
                          .DependOn<Facts::ReleaseWithdrawal>())

                // FirmAggregates
                .Aggregate<FirmAggregates::Firm>(
                    x => x.Match<Facts::Firm>()
                          .DependOn<Facts::FirmAddress>()
                          .DependOn<Facts::FirmAddressCategory>()
                          .DependOn<Facts::Order>()
                          .DependOn<Facts::OrderItem>()
                          .DependOn<Facts::OrderPosition>()
                          .DependOn<Facts::OrderPositionAdvertisement>()
                          .DependOn<Facts::Position>())
                .Aggregate<FirmAggregates::Order>(
                    x => x.Match<Facts::Order>()
                          .DependOn<Facts::Firm>()
                          .DependOn<Facts::FirmAddress>()
                          .DependOn<Facts::OrderPosition>()
                          .DependOn<Facts::OrderPositionAdvertisement>()
                          .DependOn<Facts::Position>())

                // PriceAggregates
                .Aggregate<PriceAggregates::Order>(
                    x => x.Match<Facts::Order>()
                          .DependOn<Facts::OrderPosition>()
                          .DependOn<Facts::OrderPositionAdvertisement>()
                          .DependOn<Facts::FirmAddress>()
                          .DependOn<Facts::Position>()
                          .DependOn<Facts::Price>()
                          .DependOn<Facts::PricePosition>()
                          .DependOn<Facts::Project>())
                .Aggregate<PriceAggregates::Period>(
                    x => x.Match<object>()
                          .DependOn<Facts::Order>()
                          .DependOn<Facts::Price>())
                .Aggregate<PriceAggregates::Firm>(
                    x => x.Match<Facts::Firm>()
                          .DependOn<Facts::Order>()
                          .DependOn<Facts::OrderItem>()
                          .DependOn<Facts::Category>()
                          .DependOn<Facts::Ruleset>())
                .Aggregate<PriceAggregates::Ruleset>(
                    x => x.Match<Facts::Ruleset>()
                          .DependOn<Facts::NomenclatureCategory>())

                // ProjectAggregates
                .Aggregate<ProjectAggregates::Order>(
                    x => x.Match<Facts::Order>()
                          .DependOn<Facts::Category>()
                          .DependOn<Facts::FirmAddress>()
                          .DependOn<Facts::OrderPosition>()
                          .DependOn<Facts::OrderPositionCostPerClick>()
                          .DependOn<Facts::OrderPositionAdvertisement>()
                          .DependOn<Facts::Position>()
                          .DependOn<Facts::PricePosition>()
                          .DependOn<Facts::Project>())
                .Aggregate<ProjectAggregates::Project>(
                    x => x.Match<Facts::Project>()
                          .DependOn<Facts::CostPerClickCategoryRestriction>()
                          .DependOn<Facts::SalesModelCategoryRestriction>()
                          .DependOn<Facts::CategoryOrganizationUnit>()
                          .DependOn<Facts::ReleaseInfo>())

                // ThemeAggregates
                .Aggregate<ThemeAggregates::Order>(
                    x => x.Match<Facts::Order>()
                          .DependOn<Facts::OrderPosition>()
                          .DependOn<Facts::OrderPositionAdvertisement>()
                          .DependOn<Facts::Project>())
                .Aggregate<ThemeAggregates::Project>(
                    x => x.Match<Facts::Project>()
                          .DependOn<Facts::Theme>()
                          .DependOn<Facts::ThemeOrganizationUnit>())
                .Aggregate<ThemeAggregates::Theme>(
                    x => x.Match<Facts::Theme>()
                          .DependOn<Facts::Category>()
                          .DependOn<Facts::ThemeCategory>())

                // SystemAggregates
                .Aggregate<SystemAggregates::SystemStatus>(
                    x => x.Match<Facts::SystemStatus>())

                .ToDictionary(x => x.Key, x => (IReadOnlyCollection<Type>)x.Value);

        public static bool TryGetAggregateTypes(Type factType, out IReadOnlyCollection<Type> aggregateTypes)
        {
            var key = Tuple.Create(factType, factType);
            return AggregateEventMapping.TryGetValue(key, out aggregateTypes);
        }

        public static bool TryGetRelatedAggregateTypes(Type factType, Type relatedFactType, out IReadOnlyCollection<Type> aggregateTypes)
        {
            var key = Tuple.Create(factType, relatedFactType);
            return AggregateEventMapping.TryGetValue(key, out aggregateTypes);
        }

        private static Dictionary<Tuple<Type, Type>, IList<Type>> Aggregate<TAggregate>(
            this Dictionary<Tuple<Type, Type>, IList<Type>> dictionary,
            Action<FluentDictionaryBuilder> action)
        {
            var builder = new FluentDictionaryBuilder();
            action.Invoke(builder);

            dictionary.Append(Tuple.Create(builder.Matched, builder.Matched), typeof(TAggregate));
            foreach (var depended in builder.Depended)
            {
                // Первым идёт тип, от которого зависит агрегат. Он же тип, accessor которого сгенерировал событие.
                dictionary.Append(Tuple.Create(depended, builder.Matched), typeof(TAggregate));
            }

            return dictionary;
        }

        private static void Append<TKey, TValue>(this Dictionary<TKey, IList<TValue>> dictionary, TKey key, TValue value)
        {
            if (!dictionary.TryGetValue(key, out var list))
            {
                list = new List<TValue>();
                dictionary.Add(key, list);
            }

            list.Add(value);
        }

        private sealed class FluentDictionaryBuilder
        {
            public Type Matched { get; private set; }

            public IList<Type> Depended { get; } = new List<Type>();

            public FluentDictionaryBuilder Match<T>()
            {
                if (Matched != null)
                {
                    throw new InvalidOperationException("Matched has already been set");
                }

                Matched = typeof(T);
                return this;
            }

            // todo: информацию DependOn должно быть легко вытащить из выражений, тогда не нужно будет её поддерживать вручную
            public FluentDictionaryBuilder DependOn<T>()
            {
                Depended.Add(typeof(T));
                return this;
            }
        }

    }
}