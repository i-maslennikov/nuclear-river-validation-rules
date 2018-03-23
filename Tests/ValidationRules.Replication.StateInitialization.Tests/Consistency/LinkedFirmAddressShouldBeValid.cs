using System.Collections.Generic;
using System.Linq;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Facts;
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
        private static ArrangeMetadataElement LinkedFirmAddressShouldBeValid
            => ArrangeMetadataElement
                .Config
                .Name(nameof(LinkedFirmAddressShouldBeValid))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2), FirmId = 1 },
                    new Facts::OrderPosition { Id = 1, OrderId = 1 },

                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 1, FirmAddressId = 1, PositionId = 1 },
                    new Facts::FirmAddress { Id = 1, FirmId = 2 },

                    new Facts::OrderPositionAdvertisement { Id = 2, OrderPositionId = 1, FirmAddressId = 2, PositionId = 1 },
                    new Facts::FirmAddress { Id = 2, FirmId = 1, IsDeleted = true },

                    new Facts::OrderPositionAdvertisement { Id = 3, OrderPositionId = 1, FirmAddressId = 3, PositionId = 1 },
                    new Facts::FirmAddress { Id = 3, FirmId = 1, IsActive = false },

                    new Facts::OrderPositionAdvertisement { Id = 4, OrderPositionId = 1, FirmAddressId = 4, PositionId = 1 },
                    new Facts::FirmAddress { Id = 4, FirmId = 1, IsActive = true, IsClosedForAscertainment = true },

                    new Facts::OrderPositionAdvertisement { Id = 5, OrderPositionId = 1, FirmAddressId = 5, PositionId = 2 },
                    new Facts::FirmAddress { Id = 5, FirmId = 1, IsActive = true, EntranceCode = 1, BuildingPurposeCode = 1},

                    new Facts::OrderPositionAdvertisement { Id = 6, OrderPositionId = 1, FirmAddressId = 6, PositionId = 2 },
                    new Facts::FirmAddress { Id = 6, FirmId = 1, IsActive = true, EntranceCode = null, BuildingPurposeCode = null },

                    new Facts::OrderPositionAdvertisement { Id = 7, OrderPositionId = 1, FirmAddressId = 7, PositionId = 2 },
                    new Facts::FirmAddress { Id = 7, FirmId = 1, IsActive = true, EntranceCode = 1, BuildingPurposeCode = FirmAddress.InvalidBuildingPurposeCodesForPoi.First()},

                    new Facts::Position {Id = 1},
                    new Facts::Position {Id = 2, CategoryCode = Position.CategoryCodesPoiAddressCheck.First()})
                .Aggregate(
                    new Aggregates::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.InvalidFirmAddress { OrderId = 1, FirmAddressId = 1, OrderPositionId = 1, PositionId = 1, State = Aggregates::InvalidFirmAddressState.NotBelongToFirm },
                    new Aggregates::Order.InvalidFirmAddress { OrderId = 1, FirmAddressId = 2, OrderPositionId = 1, PositionId = 1, State = Aggregates::InvalidFirmAddressState.Deleted },
                    new Aggregates::Order.InvalidFirmAddress { OrderId = 1, FirmAddressId = 3, OrderPositionId = 1, PositionId = 1, State = Aggregates::InvalidFirmAddressState.NotActive },
                    new Aggregates::Order.InvalidFirmAddress { OrderId = 1, FirmAddressId = 4, OrderPositionId = 1, PositionId = 1, State = Aggregates::InvalidFirmAddressState.ClosedForAscertainment },
                    new Aggregates::Order.InvalidFirmAddress { OrderId = 1, FirmAddressId = 6, OrderPositionId = 1, PositionId = 2, State = Aggregates::InvalidFirmAddressState.MissingEntrance },
                    new Aggregates::Order.InvalidFirmAddress { OrderId = 1, FirmAddressId = 7, OrderPositionId = 1, PositionId = 2, State = Aggregates::InvalidFirmAddressState.InvalidBuildingPurpose })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Dictionary<string, object> { { "invalidFirmAddressState", (int)Aggregates::InvalidFirmAddressState.NotBelongToFirm } },
                                    new Reference<EntityTypeFirmAddress>(1),
                                    new Reference<EntityTypeOrder>(1),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(1),
                                        new Reference<EntityTypePosition>(1)))
                                .ToXDocument(),
                            MessageType = (int)MessageTypeCode.LinkedFirmAddressShouldBeValid,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Dictionary<string, object> { { "invalidFirmAddressState", (int)Aggregates::InvalidFirmAddressState.Deleted } },
                                    new Reference<EntityTypeFirmAddress>(2),
                                    new Reference<EntityTypeOrder>(1),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(1),
                                        new Reference<EntityTypePosition>(1)))
                                .ToXDocument(),
                            MessageType = (int)MessageTypeCode.LinkedFirmAddressShouldBeValid,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        },

                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Dictionary<string, object> { { "invalidFirmAddressState", (int)Aggregates::InvalidFirmAddressState.NotActive } },
                                    new Reference<EntityTypeFirmAddress>(3),
                                    new Reference<EntityTypeOrder>(1),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(1),
                                        new Reference<EntityTypePosition>(1)))
                                .ToXDocument(),
                            MessageType = (int)MessageTypeCode.LinkedFirmAddressShouldBeValid,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        },

                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Dictionary<string, object> { { "invalidFirmAddressState", (int)Aggregates::InvalidFirmAddressState.ClosedForAscertainment } },
                                    new Reference<EntityTypeFirmAddress>(4),
                                    new Reference<EntityTypeOrder>(1),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(1),
                                        new Reference<EntityTypePosition>(1)))
                                .ToXDocument(),
                            MessageType = (int)MessageTypeCode.LinkedFirmAddressShouldBeValid,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                                            new Dictionary<string, object> { { "invalidFirmAddressState", (int)Aggregates::InvalidFirmAddressState.MissingEntrance } },
                                                            new Reference<EntityTypeFirmAddress>(6),
                                                            new Reference<EntityTypeOrder>(1),
                                                            new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                                                                                                new Reference<EntityTypeOrderPosition>(1),
                                                                                                                new Reference<EntityTypePosition>(2)))
                                .ToXDocument(),
                            MessageType = (int)MessageTypeCode.LinkedFirmAddressShouldBeValid,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                                            new Dictionary<string, object> { { "invalidFirmAddressState", (int)Aggregates::InvalidFirmAddressState.InvalidBuildingPurpose } },
                                                            new Reference<EntityTypeFirmAddress>(7),
                                                            new Reference<EntityTypeOrder>(1),
                                                            new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                                                                                                new Reference<EntityTypeOrderPosition>(1),
                                                                                                                new Reference<EntityTypePosition>(2)))
                                .ToXDocument(),
                            MessageType = (int)MessageTypeCode.LinkedFirmAddressShouldBeValid,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        });
    }
}
