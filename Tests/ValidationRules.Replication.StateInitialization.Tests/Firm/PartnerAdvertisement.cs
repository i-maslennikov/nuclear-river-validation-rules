using System.Collections.Generic;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PartnerAdvertisementShouldNotBeSoldToAdvertiser
            => ArrangeMetadataElement
                .Config
                .Name(nameof(PartnerAdvertisementShouldNotBeSoldToAdvertiser))
                .Aggregate(
                    // Заказ с змк в адрес РД
                    new Aggregates::Order { Id = 1, Begin = MonthStart(1), End = MonthStart(3), FirmId = 1, Scope = 0 },
                    new Aggregates::Order.PartnerPosition { OrderId = 1, DestinationFirmId = 2, DestinationFirmAddressId = 2, IsPremium = false },

                    // Заказ РД
                    new Aggregates::Order { Id = 2, Begin = MonthStart(2), End = MonthStart(4), FirmId = 2, Scope = 0 }
                )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                        new Reference<EntityTypeOrder>(1), // Заказ, размещающий ссылку
                                        new Reference<EntityTypeOrder>(2), // Заказ фирмы-рекламодателя (хоста)
                                        new Reference<EntityTypeFirm>(2), // Фирма-рекламодатель (хост)
                                        new Reference<EntityTypeFirmAddress>(2))
                                    .ToXDocument(),
                            MessageType = (int)MessageTypeCode.PartnerAdvertisementShouldNotBeSoldToAdvertiser,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(3),
                            OrderId = 1,
                        });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PremiumPartnerAdvertisementMustNotBeSoldToAdvertiser
            => ArrangeMetadataElement
                .Config
                .Name(nameof(PremiumPartnerAdvertisementMustNotBeSoldToAdvertiser))
                .Aggregate(
                    // Заказ с премиум-змк в адрес РД
                    new Aggregates::Order { Id = 1, Begin = MonthStart(1), End = MonthStart(3), FirmId = 1, Scope = 0 },
                    new Aggregates::Order.PartnerPosition { OrderId = 1, DestinationFirmId = 2, DestinationFirmAddressId = 2, IsPremium = true },

                    // Заказ РД
                    new Aggregates::Order { Id = 2, Begin = MonthStart(2), End = MonthStart(4), FirmId = 2, Scope = 0 }
                )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                        new Reference<EntityTypeOrder>(1), // Заказ, размещающий ссылку
                                        new Reference<EntityTypeOrder>(2), // Заказ фирмы-рекламодателя (хоста)
                                        new Reference<EntityTypeFirm>(2), // Фирма-рекламодатель (хост)
                                        new Reference<EntityTypeFirmAddress>(2))
                                    .ToXDocument(),
                            MessageType = (int)MessageTypeCode.PremiumPartnerAdvertisementMustNotBeSoldToAdvertiser,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(3),
                            OrderId = 1,
                        });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmAddressShouldNotHaveMultiplePartnerAdvertisement
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmAddressShouldNotHaveMultiplePartnerAdvertisement))
                .Aggregate(
                    // Одобренный заказ с змк в адрес третьей фирмы (не видит второго)
                    new Aggregates::Order { Id = 1, Begin = MonthStart(1), End = MonthStart(3), FirmId = 1, Scope = 0 },
                    new Aggregates::Order.PartnerPosition { OrderId = 1, DestinationFirmId = 3, DestinationFirmAddressId = 3, IsPremium = false },

                    // Заказ на оформлении с змк в адрес третьей фирмы (видит первого)
                    new Aggregates::Order { Id = 2, Begin = MonthStart(2), End = MonthStart(4), FirmId = 2, Scope = 2 },
                    new Aggregates::Order.PartnerPosition { OrderId = 2, DestinationFirmId = 3, DestinationFirmAddressId = 3, IsPremium = false }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "begin", MonthStart(2) }, { "end", MonthStart(3) } },
                                    new Reference<EntityTypeOrder>(2),
                                    new Reference<EntityTypeFirm>(3),
                                    new Reference<EntityTypeFirmAddress>(3))
                                .ToXDocument(),
                        MessageType = (int)MessageTypeCode.FirmAddressShouldNotHaveMultiplePartnerAdvertisement,
                        PeriodStart = MonthStart(2),
                        PeriodEnd = MonthStart(3),
                        OrderId = 2,
                    });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmAddressMustNotHaveMultiplePremiumPartnerAdvertisement
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmAddressMustNotHaveMultiplePremiumPartnerAdvertisement))
                .Aggregate(
                    // Одобренный заказ с премиум-змк в адрес третьей фирмы (не видит второго)
                    new Aggregates::Order { Id = 1, Begin = MonthStart(1), End = MonthStart(3), FirmId = 1, Scope = 0 },
                    new Aggregates::Order.PartnerPosition { OrderId = 1, DestinationFirmId = 3, DestinationFirmAddressId = 3, IsPremium = true },

                    // Заказ на оформлении с премиум-змк в адрес третьей фирмы (видит первого)
                    new Aggregates::Order { Id = 2, Begin = MonthStart(2), End = MonthStart(4), FirmId = 2, Scope = 2 },
                    new Aggregates::Order.PartnerPosition { OrderId = 2, DestinationFirmId = 3, DestinationFirmAddressId = 3, IsPremium = true }
                )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "begin", MonthStart(2) }, { "end", MonthStart(3) } },
                                    new Reference<EntityTypeOrder>(2),
                                    new Reference<EntityTypeFirm>(3),
                                    new Reference<EntityTypeFirmAddress>(3))
                                .ToXDocument(),
                        MessageType = (int)MessageTypeCode.FirmAddressMustNotHaveMultiplePremiumPartnerAdvertisement,
                        PeriodStart = MonthStart(2),
                        PeriodEnd = MonthStart(3),
                        OrderId = 2,
                    });
    }
}

