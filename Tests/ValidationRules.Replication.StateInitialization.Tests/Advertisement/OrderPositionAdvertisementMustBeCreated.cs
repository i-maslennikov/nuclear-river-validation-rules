using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionAdvertisementMustBeCreatedPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPositionAdvertisementMustBeCreatedPositive))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, PricePositionId = 4 },
                    new Facts::PricePosition { Id = 4, PositionId = 5},
                    new Facts::Position { Id = 5, Name = "Position5", IsCompositionOptional = false, ChildPositionId = 6 },
                    new Facts::Position { Id = 6, Name = "Position6" }

                    // no Facts::OrderPositionAdvertisement
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Aggregates::Order.MissingOrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, CompositePositionId = 5, PositionId = 6 },

                    new Aggregates::Position { Id = 5, Name = "Position5" },
                    new Aggregates::Position { Id = 6, Name = "Position6" }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><order id = \"1\" name=\"Order1\" /><orderPosition id = \"4\" name=\"Position5\" /><position id = \"6\" name=\"Position6\" /></root>"),
                        MessageType = (int)MessageTypeCode.OrderPositionAdvertisementMustBeCreated,
                        Result = 255,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionAdvertisementMustBeCreatedNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPositionAdvertisementMustBeCreatedNegative))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, PricePositionId = 4 },
                    new Facts::PricePosition { Id = 4, PositionId = 5 },
                    new Facts::Position { Id = 5, Name = "Position5", IsCompositionOptional = false, ChildPositionId = 6 },
                    new Facts::Position { Id = 6, Name = "Position6" },

                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 6, AdvertisementId = 7}
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, Number = "Order1", BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },

                    new Aggregates::Position { Id = 5, Name = "Position5" },
                    new Aggregates::Position { Id = 6, Name = "Position6" }
                )
                .Message(
                );
    }
}
