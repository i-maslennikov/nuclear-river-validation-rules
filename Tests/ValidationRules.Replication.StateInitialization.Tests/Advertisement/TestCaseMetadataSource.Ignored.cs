using NuClear.DataTest.Metamodel.Dsl;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Aggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        /// <summary>
        /// Это такой специальный "тест", чтобы среда выполнения знала про все типы. Впоследствии нужно будет удалить.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementContectSupport
            => ArrangeMetadataElement.Config
            .Name(nameof(AdvertisementContectSupport))
            .Erm(
                new Erm::Advertisement(),
                new Erm::AdvertisementElement(),
                new Erm::AdvertisementElementStatus(),
                new Erm::AdvertisementElementTemplate(),
                new Erm::AdvertisementTemplate(),
                new Erm::Firm(),
                new Erm::Order(),
                new Erm::OrderPosition(),
                new Erm::OrderPositionAdvertisement(),
                new Erm::Position(),
                new Erm::PricePosition(),
                new Erm::Project()
                )
            .Fact(
                new Facts::Order(),
                new Facts::Project(),
                new Facts::OrderPosition(),
                new Facts::OrderPositionAdvertisement(),
                new Facts::PricePosition(),
                new Facts::Position(),
                new Facts::AdvertisementTemplate(),
                new Facts::Advertisement(),
                new Facts::Firm(),
                new Facts::AdvertisementElement(),
                new Facts::AdvertisementElementTemplate())
            .Aggregate(
                new Aggregates::Advertisement(),
                new Aggregates::Advertisement.RequiredElementMissing(),
                new Aggregates::Advertisement.ElementNotPassedReview(),
                new Aggregates::AdvertisementElementTemplate(),
                new Aggregates::Firm(),
                new Aggregates::Firm.WhiteListDistributionPeriod(),
                new Aggregates::Order(),
                new Aggregates::Order.MissingAdvertisementReference(),
                new Aggregates::Order.MissingOrderPositionAdvertisement(),
                new Aggregates::Order.AdvertisementDeleted(),
                new Aggregates::Order.AdvertisementMustBelongToFirm(),
                new Aggregates::Order.AdvertisementIsDummy(),
                new Aggregates::Order.OrderAdvertisement(),
                new Aggregates::Order.LinkedProject(),
                new Aggregates::Position()
                )
            .Ignored();
    }
}
