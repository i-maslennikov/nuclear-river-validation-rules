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
        private static ArrangeMetadataElement OrderPositionAdvertisementMustHaveAdvertisementPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPositionAdvertisementMustHaveAdvertisementPositive))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionPlan = FirstDayFeb },
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, PricePositionId = 4 },
                    new Facts::PricePosition { Id = 4, PositionId = 5, IsActiveNotDeleted = true },
                    new Facts::Position { Id = 5, AdvertisementTemplateId = 6 },
                    new Facts::AdvertisementTemplate { Id = 6, IsAdvertisementRequired = true },

                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = null }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Aggregates::Order.MissingAdvertisementReference { OrderId = 1, OrderPositionId = 4, CompositePositionId = 5, PositionId = 5}
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Reference<EntityTypeOrderPosition>(4,
                                    new Reference<EntityTypeOrder>(1),
                                    new Reference<EntityTypePosition>(5)),
                                new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                    new Reference<EntityTypeOrderPosition>(4),
                                    new Reference<EntityTypePosition>(5))).ToXDocument(),
                        MessageType = (int)MessageTypeCode.OrderPositionAdvertisementMustHaveAdvertisement,
                        Result = 1022,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionAdvertisementMustHaveAdvertisementPricePositionNotActive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPositionAdvertisementMustHaveAdvertisementPricePositionNotActive))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionPlan = FirstDayFeb },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, PricePositionId = 4 },
                    // price position not active
                    new Facts::PricePosition { Id = 4, PositionId = 5, IsActiveNotDeleted = false},
                    new Facts::Position { Id = 5, AdvertisementTemplateId = 6 },
                    new Facts::AdvertisementTemplate { Id = 6, IsAdvertisementRequired = true },

                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = null }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb }
                )
                .Message();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement RequiredAdvertisementMissingNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(RequiredAdvertisementMissingNegative))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionPlan = FirstDayFeb },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, PricePositionId = 4 },
                    new Facts::PricePosition { Id = 4, PositionId = 5, IsActiveNotDeleted = true },
                    new Facts::Position { Id = 5, AdvertisementTemplateId = 6 },
                    new Facts::AdvertisementTemplate { Id = 6, IsAdvertisementRequired = true },

                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 13 }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb }
                )
                .Message(
                );
    }
}
