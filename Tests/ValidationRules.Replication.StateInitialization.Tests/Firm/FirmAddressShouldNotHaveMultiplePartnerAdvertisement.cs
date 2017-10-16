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
        private static ArrangeMetadataElement FirmAddressShouldNotHaveMultiplePartnerAdvertisement
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmAddressShouldNotHaveMultiplePartnerAdvertisement))
                .Fact(
                    // Первый из заказов ЗМК (на оформлении)
                    new Facts::Order { Id = 1, FirmId = 1, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(3), WorkflowStep = 1 },
                    new Facts::OrderPosition { Id = 21, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 121, OrderPositionId = 21, PositionId = 2, FirmAddressId = 1 },

                    // Второй из заказов ЗМК (одобрен)
                    new Facts::Order { Id = 2, FirmId = 2, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(3), WorkflowStep = 5 },
                    new Facts::OrderPosition { Id = 22, OrderId = 2 },
                    new Facts::OrderPositionAdvertisement { Id = 122, OrderPositionId = 22, PositionId = 2, FirmAddressId = 1 },

                    new Facts::Position { Id = 2, CategoryCode = CategoryCodePremiumAdvertisingAddress },

                    new Facts::FirmAddress { Id = 1, FirmId = 3 })
                .Aggregate(
                    new Aggregates::Order { Id = 1, FirmId = 1, Begin = MonthStart(1), End = MonthStart(3), Scope = 1 },
                    new Aggregates::Order.PartnerPosition { OrderId = 1, DestinationFirmAddressId = 1, DestinationFirmId = 3 },

                    new Aggregates::Order { Id = 2, FirmId = 2, Begin = MonthStart(1), End = MonthStart(3), Scope = 0 },
                    new Aggregates::Order.PartnerPosition { OrderId = 2, DestinationFirmAddressId = 1, DestinationFirmId = 3 })
                .Message(
                    new Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                        new Dictionary<string, object> { { "begin", MonthStart(1) }, { "end", MonthStart(3) } },
                                        new Reference<EntityTypeOrder>(1),
                                        new Reference<EntityTypeFirm>(3),
                                        new Reference<EntityTypeFirmAddress>(1))
                                    .ToXDocument(),

                            MessageType = (int)MessageTypeCode.FirmAddressShouldNotHaveMultiplePartnerAdvertisement,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(3),
                            OrderId = 1,
                        });
    }
}

