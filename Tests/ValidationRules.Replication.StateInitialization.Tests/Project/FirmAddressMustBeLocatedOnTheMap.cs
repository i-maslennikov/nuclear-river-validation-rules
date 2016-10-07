using System;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmAddressMustBeLocatedOnTheMap
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmAddressMustBeLocatedOnTheMap))
                .Fact(
                    new Facts::Order { Id = 1, Number = "Order", BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Facts::OrderPosition { Id = 3, OrderId = 1, PricePositionId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 3, FirmAddressId = 2, PositionId = 4 },
                    new Facts::FirmAddress { Id = 2, IsLocatedOnTheMap = false, Name = "Address" },
                    new Facts::Position { Id = 4, Name = "Position" },
                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, Number = "Order", Begin = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Order.AddressAdvertisement { OrderId = 1, AddressId = 2, MustBeLocatedOnTheMap = true, OrderPositionId = 3, PositionId = 4 },
                    new Aggregates::FirmAddress { Id = 2, IsLocatedOnTheMap = false, Name = "Address" },
                    new Aggregates::Position { Id = 4, Name = "Position" })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse(
                                "<root><firmAddress id=\"2\" name=\"Address\" /><order id=\"1\" number=\"Order\" /><orderPosition id=\"3\" name=\"Position\" /></root>"),
                            MessageType = (int)MessageTypeCode.FirmAddressMustBeLocatedOnTheMap,
                            Result = 255,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                        });

        /// <summary>
        /// Позиции "Рекламная ссылка", "Выгодные покупки с 2ГИС", "Комментарий к адресу" могут продаваться к адресам, не размещённым на карте
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmAddressMustBeLocatedOnTheMapSpecialCategoryCode
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmAddressMustBeLocatedOnTheMapSpecialCategoryCode))
                .Fact(
                    new Facts::Order { Id = 1, Number = "Order", BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Facts::OrderPosition { Id = 3, OrderId = 1, PricePositionId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 3, FirmAddressId = 2, PositionId = 4 },
                    new Facts::FirmAddress { Id = 2, IsLocatedOnTheMap = false, Name = "Address" },
                    new Facts::Position { Id = 4, Name = "Position", CategoryCode = 11 },
                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, Number = "Order", Begin = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Order.AddressAdvertisement { OrderId = 1, AddressId = 2, MustBeLocatedOnTheMap = false, OrderPositionId = 3, PositionId = 4 },
                    new Aggregates::FirmAddress { Id = 2, IsLocatedOnTheMap = false, Name = "Address" },
                    new Aggregates::Position { Id = 4, Name = "Position" })
                .Message();
    }
}
