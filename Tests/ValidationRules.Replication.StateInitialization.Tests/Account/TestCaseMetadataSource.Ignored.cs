using System;

using NuClear.DataTest.Metamodel.Dsl;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Aggregates = NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.AccountRules.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        /// <summary>
        /// Это такой специальный "тест", чтобы среда выполнения знала про все типы. Впоследствии нужно будет удалить.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AccountContectSupport
            => ArrangeMetadataElement.Config
            .Name(nameof(AccountContectSupport))
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
                new Facts::Account(),
                new Facts::Limit(),
                new Facts::Lock(),
                new Facts::Order(),
                new Facts::OrderPosition(),
                new Facts::Project(),
                new Facts::ReleaseWithdrawal())
            .Aggregate(
                new Aggregates::Account(),
                new Aggregates::AccountPeriod(),
                new Aggregates::Lock(),
                new Aggregates::Order())
            .Ignored();
    }
}
