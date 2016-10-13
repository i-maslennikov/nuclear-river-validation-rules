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

                    new Facts::AdvertisementElement { Id = 8, AdvertisementId = 6, BeginDate = FirstDayJan, EndDate = FirstDayJan }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Aggregates::Order.AdvertisementPeriodNotInOrderPeriod { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Position { Id = 5, Name = "Position5" },
                    new Aggregates::Advertisement { Id = 6, Name = "Advertisement6" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" number=\"Order1\" /><orderPosition id = \"4\" name=\"Position5\" /><advertisement id = \"6\" name=\"Advertisement6\" /></root>"),
                        MessageType = (int)MessageTypeCode.OrderPeriodMustContainAdvertisementPeriod,
                        Result = 254,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        ProjectId = 3,
                    }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPeriodMustContainAdvertisementPeriodNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPeriodMustContainAdvertisementPeriodNegative))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Position { Id = 5, Name = "Position5" },
                    new Facts::Advertisement { Id = 6, Name = "Advertisement6", AdvertisementTemplateId = 7, FirmId = 0 },
                    new Facts::AdvertisementTemplate { Id = 7 },

                    new Facts::AdvertisementElement { Id = 8, AdvertisementId = 6, BeginDate = FirstDayJan, EndDate = FirstDayFeb }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },

                    new Aggregates::Position { Id = 5, Name = "Position5" },
                    new Aggregates::Advertisement { Id = 6, Name = "Advertisement6" }
                )
                .Message(
                );
    }
}
