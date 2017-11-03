using System.Collections.Generic;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertiserMustBeNotifiedAboutPartnerAdvertisement
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AdvertiserMustBeNotifiedAboutPartnerAdvertisement))
                .Fact(
                    // Заказ ЗМК
                    new Facts::Order { Id = 1, FirmId = 1, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(3), WorkflowStep = 1 },
                    new Facts::OrderPosition { Id = 21, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 121, OrderPositionId = 21, PositionId = 2, FirmAddressId = 1 },

                    // Заказ фирмы, на адрес которой подкинули рекламу
                    new Facts::Order { Id = 3, FirmId = 3, BeginDistribution = MonthStart(2), EndDistributionFact = MonthStart(4), WorkflowStep = 5 },

                    new Facts::Position { Id = 2, CategoryCode = CategoryCodePremiumAdvertisingAddress },

                    new Facts::FirmAddress { Id = 1, FirmId = 3 })
                .Aggregate(
                    new Aggregates::Order { Id = 1, FirmId = 1, Begin = MonthStart(1), End = MonthStart(3), Scope = 1 },
                    new Aggregates::Order.PartnerPosition { OrderId = 1, DestinationFirmAddressId = 1, DestinationFirmId = 3 },

                    new Aggregates::Order { Id = 3, FirmId = 3, Begin = MonthStart(2), End = MonthStart(4), Scope = 0 })
                .Message(
                    new Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                                  new Reference<EntityTypeOrder>(3),
                                                  new Reference<EntityTypeOrder>(1),
                                                  new Reference<EntityTypeFirm>(1),
                                                  new Reference<EntityTypeFirmAddress>(1))
                                    .ToXDocument(),

                            MessageType = (int)MessageTypeCode.AdvertiserMustBeNotifiedAboutPartnerAdvertisement,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(4),
                            OrderId = 3,
                        },

                    new Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                                  new Reference<EntityTypeOrder>(3),
                                                  new Reference<EntityTypeOrder>(1),
                                                  new Reference<EntityTypeFirm>(3),
                                                  new Reference<EntityTypeFirmAddress>(1))
                                    .ToXDocument(),

                            MessageType = (int)MessageTypeCode.PartnerAdvertisementShouldNotBeSoldToAdvertiser,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(3),
                            OrderId = 1,
                        });
    }
}

