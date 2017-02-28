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
        private static ArrangeMetadataElement OrderMustHaveAdvertisementPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustHaveAdvertisementPositive))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionPlan = FirstDayFeb },
                    new Facts::Project {Id = 3, OrganizationUnitId = 2},

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Advertisement { Id = 6, FirmId = 0, AdvertisementTemplateId = 9, IsDeleted = false },
                    new Facts::AdvertisementTemplate { Id = 9, DummyAdvertisementId = -6 },
                    new Facts::AdvertisementElement { Id = 7, AdvertisementId = 6, AdvertisementElementTemplateId = 8, IsEmpty = true }, // ЭРМ пустой
                    new Facts::AdvertisementElementTemplate { Id = 8, IsRequired = true } // ЭРМ не должен быть пустым
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Advertisement { Id = 6 },
                    new Aggregates::Advertisement.RequiredElementMissing { AdvertisementId = 6, AdvertisementElementId = 7, AdvertisementElementTemplateId = 8 }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Reference<EntityTypeOrder>(1),
                                new Reference<EntityTypeAdvertisement>(6),
                                new Reference<EntityTypeAdvertisementElement>(7,
                                    new Reference<EntityTypeAdvertisementElementTemplate>(8))).ToXDocument(),
                        MessageType = (int)MessageTypeCode.OrderMustHaveAdvertisement,
                        Result = 1022,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderMustHaveAdvertisementNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustHaveAdvertisementNegative))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionPlan = FirstDayFeb },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Advertisement { Id = 6, FirmId = 0, AdvertisementTemplateId = 9, IsDeleted = false },
                    new Facts::AdvertisementTemplate { Id = 9, DummyAdvertisementId = -6 },
                    new Facts::AdvertisementElement { Id = 7, AdvertisementId = 6, AdvertisementElementTemplateId = 8, IsEmpty = false },
                    new Facts::AdvertisementElementTemplate { Id = 8, IsRequired = true } // ЭРМ не должен быть пустым
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Advertisement { Id = 6 }
                )
                .Message(
                );
    }
}
