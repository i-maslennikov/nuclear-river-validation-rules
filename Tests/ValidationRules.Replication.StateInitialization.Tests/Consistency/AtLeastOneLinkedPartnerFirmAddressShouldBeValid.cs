using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        private static ArrangeMetadataElement AtLeastOneLinkedPartnerFirmAddressShouldBeValid
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AtLeastOneLinkedPartnerFirmAddressShouldBeValid))
                .Fact(
                    // buy here with only invalid addresses
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Facts::OrderPosition { Id = 1, OrderId = 1, PricePositionId = 1},
                    new Facts::PricePosition { Id = 1, PositionId = 2 },

                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 1, FirmAddressId = 1, PositionId = 1 },
                    new Facts::FirmAddress { Id = 1, IsActive = false },

                    // buy here with invalid and valid addresses
                    new Facts::Order { Id = 2, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Facts::OrderPosition { Id = 2, OrderId = 2, PricePositionId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 2, OrderPositionId = 2, FirmAddressId = 2, PositionId = 1 },
                    new Facts::FirmAddress { Id = 2, IsActive = false },
                    new Facts::OrderPositionAdvertisement { Id = 3, OrderPositionId = 2, FirmAddressId = 3, PositionId = 1 },
                    new Facts::FirmAddress { Id = 3, IsActive = true },

                    // non buy here with only invalid addresses
                    new Facts::Order { Id = 3, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Facts::OrderPosition { Id = 3, OrderId = 3, PricePositionId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 4, OrderPositionId = 3, FirmAddressId = 4, PositionId = 3 },
                    new Facts::FirmAddress { Id = 4, IsActive = false },

                    new Facts::Position { Id = 1, CategoryCode = Facts::Position.CategoryCodePartnerAdvertisingAddress },
                    new Facts::Position { Id = 2 },
                    new Facts::Position { Id = 3 })
                .Aggregate(
                    new Aggregates::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order { Id = 2, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order { Id = 3, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.MissingValidPartnerFirmAddresses { OrderId = 1, OrderPositionId = 1, PositionId = 2 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(1,
                                                                           new Reference<EntityTypeOrder>(1),
                                                                           new Reference<EntityTypePosition>(2)))
                                .ToXDocument(),
                            MessageType = (int)MessageTypeCode.AtLeastOneLinkedPartnerFirmAddressShouldBeValid,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1
                        });
    }
}