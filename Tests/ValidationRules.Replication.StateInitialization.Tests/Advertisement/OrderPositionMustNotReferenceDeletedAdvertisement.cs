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
        private static ArrangeMetadataElement OrderPositionMustNotReferenceDeletedAdvertisementPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPositionMustNotReferenceDeletedAdvertisementPositive))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Position { Id = 5, Name = "Position5" },
                    new Facts::Advertisement { Id = 6, Name = "Advertisement6", IsDeleted = true } // РМ удалён
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Aggregates::Order.AdvertisementDeleted { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6, AdvertisementName = "Advertisement6" },

                    new Aggregates::Position { Id = 5, Name = "Position5" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" number=\"Order1\" /><orderPosition id = \"4\" name=\"Position5\" /><advertisement id = \"6\" name=\"Advertisement6\" /></root>"),
                        MessageType = (int)MessageTypeCode.OrderPositionMustNotReferenceDeletedAdvertisement,
                        Result = 255,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionMustNotReferenceDeletedAdvertisementNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPositionMustNotReferenceDeletedAdvertisementNegative))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Position { Id = 5, Name = "Position5" },
                    new Facts::Advertisement { Id = 6, Name = "Advertisement6", IsDeleted = false, FirmId = 7 }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },

                    new Aggregates::Position { Id = 5, Name = "Position5" }
                )
                .Message(
                );
    }
}
