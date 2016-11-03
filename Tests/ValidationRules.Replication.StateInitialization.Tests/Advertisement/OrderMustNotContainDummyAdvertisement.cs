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
        private static ArrangeMetadataElement OrderMustNotContainDummyAdvertisementPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustNotContainDummyAdvertisementPositive))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Advertisement { Id = 6, AdvertisementTemplateId = 7 },
                    new Facts::AdvertisementTemplate { Id = 7, DummyAdvertisementId = 6 }, // Id заглушки совпадает с Id РМ

                    new Facts::Position { Id = 5, Name = "Position5" }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Aggregates::Order.AdvertisementIsDummy { OrderId = 1, OrderPositionId = 4, PositionId = 5 },

                    new Aggregates::Position { Id = 5, Name = "Position5" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" number=\"Order1\" /><orderPosition id = \"4\" name=\"Position5\" /></root>"),
                        MessageType = (int)MessageTypeCode.OrderMustNotContainDummyAdvertisement,
                        Result = 250,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderMustNotContainDummyAdvertisementNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustNotContainDummyAdvertisementNegative))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::AdvertisementTemplate { Id = 7, DummyAdvertisementId = 7 },

                    new Facts::Position { Id = 5, Name = "Position5" }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },

                    new Aggregates::Position { Id = 5, Name = "Position5" }
                )
                .Message(
                );
    }
}
