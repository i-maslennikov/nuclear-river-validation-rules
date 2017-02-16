using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Aggregates = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;

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
                new Erm::BranchOffice(),
                new Erm::BranchOfficeOrganizationUnit(),
                new Erm::Category(),
                new Erm::CategoryFirmAddress(),
                new Erm::Deal(),
                new Erm::DeniedPosition(),
                new Erm::Firm(),
                new Erm::FirmAddress(),
                new Erm::LegalPerson(),
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
                new Facts::Bargain(),
                new Facts::BargainScanFile(),
                new Facts::Bill(),
                new Facts::BranchOffice(),
                new Facts::BranchOfficeOrganizationUnit(),
                new Facts::Category(),
                new Facts::FirmAddressCategory(),
                new Facts::Deal(),
                new Facts::Firm(),
                new Facts::FirmAddress(),
                new Facts::LegalPerson(),
                new Facts::LegalPersonProfile(),
                new Facts::Order(),
                new Facts::OrderPosition(),
                new Facts::OrderPositionAdvertisement(),
                new Facts::OrderScanFile(),
                new Facts::Position(),
                new Facts::Project(),
                new Facts::ReleaseWithdrawal())
            .Aggregate(
                new Aggregates::Order(),
                new Aggregates::Order.BargainSignedLaterThanOrder(),
                new Aggregates::Order.HasNoAnyLegalPersonProfile(),
                new Aggregates::Order.HasNoAnyPosition(),
                new Aggregates::Order.InactiveReference(),
                new Aggregates::Order.InvalidBeginDistributionDate(),
                new Aggregates::Order.InvalidBillsPeriod(),
                new Aggregates::Order.InvalidBillsTotal(),
                new Aggregates::Order.InvalidCategory(),
                new Aggregates::Order.CategoryNotBelongsToAddress(),
                new Aggregates::Order.InvalidEndDistributionDate(),
                new Aggregates::Order.InvalidFirm(),
                new Aggregates::Order.InvalidFirmAddress(),
                new Aggregates::Order.LegalPersonProfileBargainExpired(),
                new Aggregates::Order.LegalPersonProfileWarrantyExpired(),
                new Aggregates::Order.MissingBargainScan(),
                new Aggregates::Order.MissingBills(),
                new Aggregates::Order.MissingOrderScan(),
                new Aggregates::Order.MissingRequiredField())
            .Ignored();
    }
}
