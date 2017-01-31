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
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayMay },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId =  4, PositionId = 5, AdvertisementId = 6 },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 7 },

                    new Aggregates::Advertisement { Id = 6, },
                    new Aggregates::Advertisement.Coupon { AdvertisementId = 6, AdvertisementElementId = 8, DaysTotal = 5, DaysFromMonthBeginToCouponEnd = 5, DaysFromCouponBeginToMonthEnd = 5, BeginMonth = FirstDayJan, EndMonth = FirstDayFeb },
                    new Aggregates::Advertisement { Id = 7, },
                    new Aggregates::Advertisement.Coupon { AdvertisementId = 7, AdvertisementElementId = 9, DaysTotal = 5, DaysFromMonthBeginToCouponEnd = 5, DaysFromCouponBeginToMonthEnd = 5, BeginMonth = FirstDayMar, EndMonth = FirstDayApr }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" /><opa><orderPosition id = \"4\" /><position id = \"5\" /></opa><advertisement id = \"6\" /></root>"),
                        MessageType = (int)MessageTypeCode.OrderCouponPeriodMustBeInRelease,
                        Result = 252,
                        PeriodStart = FirstDayFeb,
                        PeriodEnd = FirstDayMay,
                        OrderId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" /><opa><orderPosition id = \"4\" /><position id = \"5\" /></opa><advertisement id = \"7\" /></root>"),
                        MessageType = (int)MessageTypeCode.OrderCouponPeriodMustBeInRelease,
                        Result = 252,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayMar,
                        OrderId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" /><opa><orderPosition id = \"4\" /><position id = \"5\" /></opa><advertisement id = \"7\" /></root>"),
                        MessageType = (int)MessageTypeCode.OrderCouponPeriodMustBeInRelease,
                        Result = 252,
                        PeriodStart = FirstDayApr,
                        PeriodEnd = FirstDayMay,
                        OrderId = 1,
                    }
                );
    }
}
