using System;

using NuClear.DataTest.Metamodel.Dsl;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Aggregates = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        /// <summary>
        /// Это такой специальный "тест", чтобы среда выполнения знала про все типы. Впоследствии нужно будет удалить.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ConsistencyContextSupport
            => ArrangeMetadataElement.Config
            .Name(nameof(ConsistencyContextSupport))
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
                new Erm::Limit(),
                new Erm::Lock(),
                new Erm::Order(),
                new Erm::OrderFile(),
                new Erm::OrderPosition(),
                new Erm::OrderPositionAdvertisement(),
                new Erm::OrganizationUnit(),
                new Erm::Position(),
                new Erm::Price(),
                new Erm::PricePosition(),
                new Erm::Project(),
                new Erm::ReleaseInfo(),
                new Erm::ReleaseWithdrawal(),
                new Erm::Ruleset(),
                new Erm::RulesetRule(),
                new Erm::Theme(),
                new Erm::TimeZone(),
                new Erm::User(),
                new Erm::UserProfile())
            .Fact(
                new Facts::Bargain(),
                new Facts::BargainScanFile(),
                new Facts::Bill(),
                new Facts::Category(),
                new Facts::CategoryFirmAddress(),
                new Facts::Firm(),
                new Facts::FirmAddress(),
                new Facts::LegalPersonProfile(),
                new Facts::Order(),
                new Facts::OrderPosition(),
                new Facts::OrderPositionAdvertisement(),
                new Facts::OrderScanFile(),
                new Facts::Position(),
                new Facts::Project())
            .Aggregate(
                new Aggregates::Order(),
                new Aggregates::Order.BargainSignedLaterThanOrder(),
                new Aggregates::Order.HasNoAnyLegalPersonProfile(),
                new Aggregates::Order.HasNoAnyPosition(),
                new Aggregates::Order.InvalidBeginDistributionDate(),
                new Aggregates::Order.InvalidBillsPeriod(),
                new Aggregates::Order.InvalidBillsTotal(),
                new Aggregates::Order.InvalidCategory(),
                new Aggregates::Order.InvalidCategoryFirmAddress(),
                new Aggregates::Order.InvalidEndDistributionDate(),
                new Aggregates::Order.InvalidFirm(),
                new Aggregates::Order.InvalidFirmAddress(),
                new Aggregates::Order.LegalPersonProfileBargainExpired(),
                new Aggregates::Order.LegalPersonProfileWarrantyExpired(),
                new Aggregates::Order.MissingBargainScan(),
                new Aggregates::Order.MissingBills(),
                new Aggregates::Order.MissingOrderScan(),
                new Aggregates::Order.MissingRequiredField(),
                new Aggregates::Order.NoReleasesSheduled())
            .Ignored();
    }
}
