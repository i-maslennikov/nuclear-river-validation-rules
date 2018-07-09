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
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement LinkedCategoryFirmAddressShouldBeValid
            => ArrangeMetadataElement
                .Config
                .Name(nameof(LinkedCategoryFirmAddressShouldBeValid))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Facts::OrderPosition { Id = 1, OrderId = 1 },

                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 1, CategoryId = 2, FirmAddressId = 2 },
                    new Facts::FirmAddress { Id = 2, IsActive = true },
                    new Facts::FirmAddressCategory { Id = 2, FirmAddressId = 2, CategoryId = 2 },

                    new Facts::OrderPositionAdvertisement { Id = 2, OrderPositionId = 1, CategoryId = 3, FirmAddressId = 3 },
                    new Facts::FirmAddress { Id = 3, IsActive = true })
                .Aggregate(
                    new Aggregates::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.CategoryNotBelongsToAddress { OrderId = 1, CategoryId = 3, FirmAddressId = 3, OrderPositionId = 1 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Reference<EntityTypeFirmAddress>(3),
                                    new Reference<EntityTypeCategory>(3),
                                    new Reference<EntityTypeOrder>(1),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(1),
                                        new Reference<EntityTypePosition>(0)))
                                .ToXDocument(),
                            MessageType = (int)MessageTypeCode.LinkedCategoryFirmAddressShouldBeValid,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        });
    }
}
