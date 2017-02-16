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
        private static ArrangeMetadataElement OrderPositionMustNotReferenceDeletedAdvertisementPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPositionMustNotReferenceDeletedAdvertisementPositive))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionPlan = FirstDayFeb },
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Position { Id = 5 },
                    new Facts::Advertisement { Id = 6, IsDeleted = true } // РМ удалён
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Aggregates::Order.AdvertisementDeleted { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 }
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
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionPlan = FirstDayFeb },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Position { Id = 5 },
                    new Facts::Advertisement { Id = 6, IsDeleted = false, FirmId = 7 }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb }
                )
                .Message(
                );
    }
}
