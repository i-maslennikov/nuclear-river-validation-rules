using System;

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
        private static ArrangeMetadataElement WhiteListAdvertisementMustPresent
            => ArrangeMetadataElement
                .Config
                .Name(nameof(WhiteListAdvertisementMustPresent))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayFeb, EndDistributionPlan = FirstDayFeb, FirmId = 7 },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Advertisement { Id = 6, FirmId = 7, AdvertisementTemplateId = 8, IsSelectedToWhiteList = false }, // РМ не выбран в белый список
                    new Facts::Firm { Id = 7 },
                    new Facts::AdvertisementTemplate { Id = 8, IsAllowedToWhiteList = true } // должен быть РМ в белом списке
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb, EndDistributionDateFact = FirstDayFeb, FirmId = 7, RequireWhiteListAdvertisement = true },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Firm.WhiteListDistributionPeriod { FirmId = 7, Start = FirstDayJan, End = FirstDayFeb, AdvertisementId = null, ProvidedByOrderId = null },

                    new Aggregates::Advertisement { Id = 6, FirmId = 7, IsSelectedToWhiteList = false },
                    new Aggregates::Firm { Id = 7 }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Reference<EntityTypeFirm>(7)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.WhiteListAdvertisementMustPresent,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement WhiteListAdvertisementMayPresent
            => ArrangeMetadataElement
                .Config
                .Name(nameof(WhiteListAdvertisementMayPresent))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayFeb, EndDistributionPlan = FirstDayFeb, FirmId = 7 },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },

                    new Facts::OrderPosition { Id = 4, OrderId = 1, },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Facts::Advertisement { Id = 6, FirmId = 7, AdvertisementTemplateId = 8, IsSelectedToWhiteList = true }, // РМ выбран в белый список
                    new Facts::Firm { Id = 7 },
                    new Facts::AdvertisementTemplate { Id = 8, IsAllowedToWhiteList = true } // должен быть РМ в белом списке
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDatePlan = FirstDayFeb, EndDistributionDateFact = FirstDayFeb, FirmId = 7, RequireWhiteListAdvertisement = true, ProvideWhiteListAdvertisementId = 6 },
                    new Aggregates::Order.OrderPositionAdvertisement { OrderId = 1, OrderPositionId = 4, PositionId = 5, AdvertisementId = 6 },

                    new Aggregates::Firm.WhiteListDistributionPeriod { FirmId = 7, Start = FirstDayJan, End = FirstDayFeb, AdvertisementId = null, ProvidedByOrderId = null },

                    new Aggregates::Advertisement { Id = 6, FirmId = 7, IsSelectedToWhiteList = true },
                    new Aggregates::Firm { Id = 7 }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                new Reference<EntityTypeFirm>(7),
                                new Reference<EntityTypeAdvertisement>(6)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.WhiteListAdvertisementMayPresent,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    }
                );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement WhiteListAdvertisementMayAndMustPresent
            => ArrangeMetadataElement
                .Config
                .Name(nameof(WhiteListAdvertisementMayAndMustPresent))
                .Aggregate(
                    new Aggregates::Order { Id = 1, BeginDistributionDate = MonthStart(1), EndDistributionDatePlan = MonthStart(3), RequireWhiteListAdvertisement = true },

                    new Aggregates::Firm.WhiteListDistributionPeriod { Start = DateTime.MinValue, End = MonthStart(1), ProvidedByOrderId = null, AdvertisementId = null },
                    new Aggregates::Firm.WhiteListDistributionPeriod { Start = MonthStart(1), End = MonthStart(2), ProvidedByOrderId = 111, AdvertisementId = 111 },
                    new Aggregates::Firm.WhiteListDistributionPeriod { Start = MonthStart(2), End = DateTime.MaxValue, ProvidedByOrderId = null, AdvertisementId = null },

                    new Aggregates::Advertisement { IsSelectedToWhiteList = true },
                    new Aggregates::Firm { }
                )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Reference<EntityTypeFirm>(0),
                                    new Reference<EntityTypeAdvertisement>(111)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.WhiteListAdvertisementMayPresent,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                        new Reference<EntityTypeFirm>(0)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.WhiteListAdvertisementMustPresent,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                            OrderId = 1,
                        });
    }
}
