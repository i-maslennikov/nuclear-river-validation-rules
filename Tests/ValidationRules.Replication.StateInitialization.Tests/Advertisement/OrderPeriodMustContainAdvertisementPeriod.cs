using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

using Messages = NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPeriodMustContainAdvertisementPeriodPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPeriodMustContainAdvertisementPeriodPositive))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Position { Id = 5, Name = "Position5" },
                    new Facts::Advertisement { Id = 6, Name = "Advertisement6", AdvertisementTemplateId = 7, FirmId = 0 },
                    new Facts::AdvertisementTemplate { Id = 7 },

                    new Facts::AdvertisementElement { Id = 8, AdvertisementId = 6, BeginDate = FirstDayJan.AddDays(10), EndDate = FirstDayJan.AddDays(10) },
                    new Facts::AdvertisementElement { Id = 9, AdvertisementId = 6, BeginDate = FirstDayJan.AddDays(-31), EndDate = FirstDayJan },
                    new Facts::AdvertisementElement { Id = 10, AdvertisementId = 6, BeginDate = FirstDayJan.AddDays(-1), EndDate = FirstDayFeb.AddDays(-1) },
                    new Facts::AdvertisementElement { Id = 11, AdvertisementId = 6, BeginDate = FirstDayJan, EndDate = FirstDayJan.AddDays(5) }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId =  4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Position { Id = 5, Name = "Position5" },
                    new Aggregates::Advertisement { Id = 6, Name = "Advertisement6" },
                    new Aggregates::Advertisement.ElementOffsetInDays { AdvertisementId = 6, AdvertisementElementId = 8, EndToBeginOffset = 1, EndToMonthBeginOffset = 11, MonthEndToBeginOffset = 21 },
                    new Aggregates::Advertisement.ElementOffsetInDays { AdvertisementId = 6, AdvertisementElementId = 9, EndToBeginOffset = 32, EndToMonthBeginOffset = 1, MonthEndToBeginOffset = 31 },
                    new Aggregates::Advertisement.ElementOffsetInDays { AdvertisementId = 6, AdvertisementElementId = 10, EndToBeginOffset = 32, EndToMonthBeginOffset = 31, MonthEndToBeginOffset = 1 },
                    new Aggregates::Advertisement.ElementOffsetInDays { AdvertisementId = 6, AdvertisementElementId = 11, EndToBeginOffset = 6, EndToMonthBeginOffset = 6, MonthEndToBeginOffset = 31 }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" number=\"Order1\" /><orderPosition id = \"4\" name=\"Position5\" /><advertisement id = \"6\" name=\"Advertisement6\" /></root>"),
                        MessageType = (int)MessageTypeCode.OrderPeriodMustContainAdvertisementPeriod,
                        Result = 254,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" number=\"Order1\" /><orderPosition id = \"4\" name=\"Position5\" /><advertisement id = \"6\" name=\"Advertisement6\" /></root>"),
                        MessageType = (int)MessageTypeCode.OrderPeriodMustContainAdvertisementPeriod,
                        Result = 254,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" number=\"Order1\" /><orderPosition id = \"4\" name=\"Position5\" /><advertisement id = \"6\" name=\"Advertisement6\" /></root>"),
                        MessageType = (int)MessageTypeCode.OrderPeriodMustContainAdvertisementPeriod,
                        Result = 254,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    }
                );
    }
}
