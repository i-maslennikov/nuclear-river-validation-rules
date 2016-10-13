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
        private static ArrangeMetadataElement CouponMustBeUniqueForFirm
            => ArrangeMetadataElement
                .Config
                .Name(nameof(CouponMustBeUniqueForFirm))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb, FirmId = 7 },
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::OrderPosition { Id = 5, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { Id = 2, OrderPositionId = 5, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Position { Id = 5, Name = "Position5", CategoryCode = 14},

                    new Facts::Advertisement { Id = 6, Name = "Advertisement6", FirmId = 7, AdvertisementTemplateId = 9},
                    new Facts::AdvertisementTemplate { Id = 9, DummyAdvertisementId = -6 }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb, FirmId = 7 },
                    new Aggregates::Order.CouponOrderPosition { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },
                    new Aggregates::Order.CouponOrderPosition { OrderId = 1, OrderPositionId = 5, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Advertisement { Id = 6, Name = "Advertisement6", FirmId = 7 },

                    new Aggregates::Position { Id = 5, Name = "Position5" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" number=\"Order1\" /><orderPosition id = \"4\" name=\"Position5\" /><advertisement id = \"6\" name=\"Advertisement6\" /></root>"),
                        MessageType = (int)MessageTypeCode.CouponMustBeUniqueForFirm,
                        Result = 255,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        ProjectId = 3,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" number=\"Order1\" /><orderPosition id = \"5\" name=\"Position5\" /><advertisement id = \"6\" name=\"Advertisement6\" /></root>"),
                        MessageType = (int)MessageTypeCode.CouponMustBeUniqueForFirm,
                        Result = 255,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        ProjectId = 3,
                    }
                );
    }
}
