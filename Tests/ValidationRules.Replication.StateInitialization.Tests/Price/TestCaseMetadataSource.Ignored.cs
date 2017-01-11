using NuClear.DataTest.Metamodel.Dsl;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Aggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        /// <summary>
        /// Это такой специальный "тест", чтобы среда выполнения знала про все типы. Впоследствии нужно будет удалить.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PriceContextSupport
            => ArrangeMetadataElement.Config
            .Name(nameof(PriceContextSupport))
            .Erm(
                new Erm::Account(),
                new Erm::AssociatedPosition(),
                new Erm::AssociatedPositionsGroup(),
                new Erm::Bargain(),
                new Erm::BargainFile(),
                new Erm::Bill(),
                new Erm::Category(),
                new Erm::CategoryFirmAddress(),
                new Erm::DeniedPosition(),
                new Erm::Firm(),
                new Erm::FirmAddress(),
                new Erm::LegalPersonProfile(),
                new Erm::Lock(),
                new Erm::Order(),
                new Erm::OrderFile(),
                new Erm::OrderPosition(),
                new Erm::OrderPositionAdvertisement(),
                new Erm::Position(),
                new Erm::Price(),
                new Erm::PricePosition(),
                new Erm::Project(),
                new Erm::ReleaseInfo(),
                new Erm::ReleaseWithdrawal(),
                new Erm::Ruleset(),
                new Erm::RulesetRule(),
                new Erm::Theme())
            .Fact(
                new Facts::AssociatedPosition(),
                new Facts::AssociatedPositionsGroup(),
                new Facts::Category(),
                new Facts::DeniedPosition(),
                new Facts::Order(),
                new Facts::OrderPosition(),
                new Facts::OrderPositionAdvertisement(),
                new Facts::Position(),
                new Facts::Price(),
                new Facts::PricePosition(),
                new Facts::Project(),
                new Facts::RulesetRule(),
                new Facts::Theme())
            .Aggregate(
                new Aggregates::Price.AdvertisementAmountRestriction(),
                new Aggregates::Order.AmountControlledPosition(),
                new Aggregates::Price.AssociatedPositionGroupOvercount(),
                new Aggregates::Category(),
                new Aggregates::Order(),
                new Aggregates::Order.OrderAssociatedPosition(),
                new Aggregates::Order.OrderDeniedPosition(),
                new Aggregates::Period.OrderPeriod(),
                new Aggregates::Order.OrderPosition(),
                new Aggregates::Order.OrderPricePosition(),
                new Aggregates::Period(),
                new Aggregates::Position(),
                new Aggregates::Price(),
                new Aggregates::Period.PricePeriod(),
                new Aggregates::Project(),
                new Aggregates::Theme())
            .Ignored();
    }
}
