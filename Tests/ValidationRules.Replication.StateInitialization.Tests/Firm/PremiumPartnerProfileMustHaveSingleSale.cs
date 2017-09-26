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
        const long CategoryCodePremiumAdvertising = 809065011136692321; // Реклама в профилях партнеров (приоритетное размещение)
        const long CategoryCodePremiumAdvertisingAddress = 809065011136692326; // Реклама в профилях партнеров (адреса)

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PremiumPartnerProfileMustHaveSingleSale
            => ArrangeMetadataElement
                .Config
                .Name(nameof(PremiumPartnerProfileMustHaveSingleSale))
                .Fact(
                    // Первый из конфликтующих заказов (на оформлении, не может быть одобрен)
                    new Facts::Order { Id = 1, FirmId = 1, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(2), WorkflowStep = 1 },
                    new Facts::OrderPosition { Id = 11, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 111, OrderPositionId = 11, PositionId = 1 },
                    new Facts::OrderPosition { Id = 21, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 121, OrderPositionId = 21, PositionId = 2, FirmAddressId = 1 },

                    // Второй из конфликтующих заказов (одобрен)
                    new Facts::Order { Id = 2, FirmId = 2, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(2), WorkflowStep = 5 },
                    new Facts::OrderPosition { Id = 12, OrderId = 2 },
                    new Facts::OrderPositionAdvertisement { Id = 112, OrderPositionId = 12, PositionId = 1 },
                    new Facts::OrderPosition { Id = 22, OrderId = 2 },
                    new Facts::OrderPositionAdvertisement { Id = 122, OrderPositionId = 22, PositionId = 2, FirmAddressId = 1 },

                    // Заказ на тот же адрес, но в другое время
                    new Facts::Order { Id = 3, FirmId = 3, BeginDistribution = MonthStart(2), EndDistributionFact = MonthStart(3), WorkflowStep = 5 },
                    new Facts::OrderPosition { Id = 13, OrderId = 3 },
                    new Facts::OrderPositionAdvertisement { Id = 113, OrderPositionId = 13, PositionId = 1 },
                    new Facts::OrderPosition { Id = 23, OrderId = 3 },
                    new Facts::OrderPositionAdvertisement { Id = 123, OrderPositionId = 23, PositionId = 2, FirmAddressId = 1 },

                    // Заказ на другой адрес
                    new Facts::Order { Id = 4, FirmId = 4, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(2), WorkflowStep = 5 },
                    new Facts::OrderPosition { Id = 14, OrderId = 4 },
                    new Facts::OrderPositionAdvertisement { Id = 114, OrderPositionId = 14, PositionId = 1 },
                    new Facts::OrderPosition { Id = 24, OrderId = 4 },
                    new Facts::OrderPositionAdvertisement { Id = 124, OrderPositionId = 24, PositionId = 2, FirmAddressId = 2 },

                    // Заказ без премиум-позиции
                    new Facts::Order { Id = 5, FirmId = 5, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(2), WorkflowStep = 5 },
                    new Facts::OrderPosition { Id = 25, OrderId = 5 },
                    new Facts::OrderPositionAdvertisement { Id = 125, OrderPositionId = 25, PositionId = 2, FirmAddressId = 1 },

                    new Facts::Position { Id = 1, CategoryCode = CategoryCodePremiumAdvertising },
                    new Facts::Position { Id = 2, CategoryCode = CategoryCodePremiumAdvertisingAddress },

                    new Facts::FirmAddress { Id = 1, FirmId = 1111 },
                    new Facts::FirmAddress { Id = 2, FirmId = 2222 })
                .Aggregate(
                    new Aggregates::Order { Id = 1, FirmId = 1, Begin = MonthStart(1), End = MonthStart(2), Scope = 1 },
                    new Aggregates::Order.PartnerProfilePosition { OrderId = 1, FirmAddressId = 1, FirmId = 1111, IsPremium = true },

                    new Aggregates::Order { Id = 2, FirmId = 2, Begin = MonthStart(1), End = MonthStart(2), Scope = 0 },
                    new Aggregates::Order.PartnerProfilePosition { OrderId = 2, FirmAddressId = 1, FirmId = 1111, IsPremium = true },

                    new Aggregates::Order { Id = 3, FirmId = 3, Begin = MonthStart(2), End = MonthStart(3), Scope = 0 },
                    new Aggregates::Order.PartnerProfilePosition { OrderId = 3, FirmAddressId = 1, FirmId = 1111, IsPremium = true },

                    new Aggregates::Order { Id = 4, FirmId = 4, Begin = MonthStart(1), End = MonthStart(2), Scope = 0 },
                    new Aggregates::Order.PartnerProfilePosition { OrderId = 4, FirmAddressId = 2, FirmId = 2222, IsPremium = true },

                    new Aggregates::Order { Id = 5, FirmId = 5, Begin = MonthStart(1), End = MonthStart(2), Scope = 0 },
                    new Aggregates::Order.PartnerProfilePosition { OrderId = 5, FirmAddressId = 1, FirmId = 1111, IsPremium = false })
                .Message(
                    new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "begin", MonthStart(1) }, { "end", MonthStart(2) } },
                                    new Reference<EntityTypeFirm>(1111),
                                    new Reference<EntityTypeFirmAddress>(1))
                                .ToXDocument(),

                        MessageType = (int)MessageTypeCode.PremiumPartnerProfileMustHaveSingleSale,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(2),
                        OrderId = 1,
                    });
    }
}

