using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderCouponPeriodMustBeInReleasePositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderCouponPeriodMustBeInReleasePositive))
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayMay },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId =  4, PositionId = 5, AdvertisementId = 6 },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 7 },

                    new Aggregates::Position { Id = 5, Name = "Position5" },
                    new Aggregates::Advertisement { Id = 6, Name = "Advertisement6" },
                    new Aggregates::Advertisement.ElementOffsetInDays { AdvertisementId = 6, AdvertisementElementId = 8, EndToBeginOffset = 5, EndToMonthBeginOffset = 5, MonthEndToBeginOffset = 5, BeginMonth = FirstDayJan, EndMonth = FirstDayFeb },
                    new Aggregates::Advertisement { Id = 7, Name = "Advertisement7" },
                    new Aggregates::Advertisement.ElementOffsetInDays { AdvertisementId = 7, AdvertisementElementId = 9, EndToBeginOffset = 5, EndToMonthBeginOffset = 5, MonthEndToBeginOffset = 5, BeginMonth = FirstDayMar, EndMonth = FirstDayApr }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" name=\"Order1\" /><orderPosition id = \"4\" name=\"Position5\" /><advertisement id = \"6\" name=\"Advertisement6\" /></root>"),
                        MessageType = (int)MessageTypeCode.OrderCouponPeriodMustBeInRelease,
                        Result = 252,
                        PeriodStart = FirstDayFeb,
                        PeriodEnd = FirstDayMay,
                        OrderId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" name=\"Order1\" /><orderPosition id = \"4\" name=\"Position5\" /><advertisement id = \"7\" name=\"Advertisement7\" /></root>"),
                        MessageType = (int)MessageTypeCode.OrderCouponPeriodMustBeInRelease,
                        Result = 252,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayMar,
                        OrderId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" name=\"Order1\" /><orderPosition id = \"4\" name=\"Position5\" /><advertisement id = \"7\" name=\"Advertisement7\" /></root>"),
                        MessageType = (int)MessageTypeCode.OrderCouponPeriodMustBeInRelease,
                        Result = 252,
                        PeriodStart = FirstDayApr,
                        PeriodEnd = FirstDayMay,
                        OrderId = 1,
                    }
                );
    }
}
