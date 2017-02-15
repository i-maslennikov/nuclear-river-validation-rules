using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderCouponPeriodInReleaseMustNotBeLessFiveDaysPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderCouponPeriodInReleaseMustNotBeLessFiveDaysPositive))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionPlan = FirstDayFeb },
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Position { Id = 5 },
                    new Facts::Advertisement { Id = 6, AdvertisementTemplateId = 7, FirmId = 0 },
                    new Facts::AdvertisementTemplate { Id = 7 },

                    new Facts::AdvertisementElement { Id = 8, AdvertisementId = 6, BeginDate = FirstDayJan.AddDays(10), EndDate = FirstDayJan.AddDays(10) },
                    new Facts::AdvertisementElement { Id = 9, AdvertisementId = 6, BeginDate = FirstDayDec, EndDate = FirstDayJan },
                    new Facts::AdvertisementElement { Id = 10, AdvertisementId = 6, BeginDate = FirstDayJan.AddDays(-1), EndDate = FirstDayFeb.AddDays(-1) },
                    new Facts::AdvertisementElement { Id = 11, AdvertisementId = 6, BeginDate = FirstDayJan, EndDate = FirstDayJan.AddDays(5) }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId =  4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Advertisement { Id = 6, },
                    new Aggregates::Advertisement.Coupon { AdvertisementId = 6, AdvertisementElementId = 8, DaysTotal = 1, DaysFromMonthBeginToCouponEnd = 11, DaysFromCouponBeginToMonthEnd = 21, BeginMonth = FirstDayJan, EndMonth = FirstDayFeb },
                    new Aggregates::Advertisement.Coupon { AdvertisementId = 6, AdvertisementElementId = 9, DaysTotal = 32, DaysFromMonthBeginToCouponEnd = 1, DaysFromCouponBeginToMonthEnd = 31, BeginMonth = FirstDayDec, EndMonth = FirstDayJan },
                    new Aggregates::Advertisement.Coupon { AdvertisementId = 6, AdvertisementElementId = 10, DaysTotal = 32, DaysFromMonthBeginToCouponEnd = 31, DaysFromCouponBeginToMonthEnd = 1, BeginMonth = FirstDayDec, EndMonth = FirstDayFeb },
                    new Aggregates::Advertisement.Coupon { AdvertisementId = 6, AdvertisementElementId = 11, DaysTotal = 6, DaysFromMonthBeginToCouponEnd = 6, DaysFromCouponBeginToMonthEnd = 31, BeginMonth = FirstDayJan, EndMonth = FirstDayFeb }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Reference<EntityTypeOrder>(1),
                                new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                    new Reference<EntityTypeOrderPosition>(4),
                                    new Reference<EntityTypePosition>(5)),
                                new Reference<EntityTypeAdvertisement>(6)).ToXDocument(),

                        MessageType = (int)MessageTypeCode.OrderCouponPeriodInReleaseMustNotBeLessFiveDays,
                        Result = 254,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Reference<EntityTypeOrder>(1),
                                new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                    new Reference<EntityTypeOrderPosition>(4),
                                    new Reference<EntityTypePosition>(5)),
                                new Reference<EntityTypeAdvertisement>(6)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.OrderCouponPeriodMustBeInRelease,
                        Result = 252,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Reference<EntityTypeOrder>(1),
                                new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                    new Reference<EntityTypeOrderPosition>(4),
                                    new Reference<EntityTypePosition>(5)),
                                new Reference<EntityTypeAdvertisement>(6)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.OrderCouponPeriodInReleaseMustNotBeLessFiveDays,
                        Result = 254,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Reference<EntityTypeOrder>(1),
                                new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                    new Reference<EntityTypeOrderPosition>(4),
                                    new Reference<EntityTypePosition>(5)),
                                new Reference<EntityTypeAdvertisement>(6)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.OrderCouponPeriodInReleaseMustNotBeLessFiveDays,
                        Result = 254,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    }
                );
    }
}
