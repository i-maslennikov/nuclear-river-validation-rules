using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmAddressMustBeLocatedOnTheMap
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmAddressMustBeLocatedOnTheMap))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Facts::OrderPosition { Id = 3, OrderId = 1, PricePositionId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 3, FirmAddressId = 2, PositionId = 4 },
                    new Facts::OrderPositionAdvertisement { Id = 2, OrderPositionId = 3, FirmAddressId = 2, PositionId = 5 },
                    new Facts::OrderPositionAdvertisement { Id = 3, OrderPositionId = 3, FirmAddressId = 3, PositionId = 4 },
                    new Facts::OrderPositionAdvertisement { Id = 4, OrderPositionId = 3, FirmAddressId = 3, PositionId = 5 },
                    new Facts::OrderPositionAdvertisement { Id = 5, OrderPositionId = 3, FirmAddressId = 4, PositionId = 4 },
                    new Facts::OrderPositionAdvertisement { Id = 6, OrderPositionId = 3, FirmAddressId = 4, PositionId = 5 },


                    new Facts::FirmAddress { Id = 2, IsLocatedOnTheMap = false, IsActive = true },
                    new Facts::FirmAddress { Id = 3, IsLocatedOnTheMap = false, IsActive = false },
                    new Facts::FirmAddress { Id = 4, IsLocatedOnTheMap = true, IsActive = true },

                    new Facts::Position { Id = 4 },
                    new Facts::Position { Id = 5, CategoryCode = 11 }, // Позиции "Рекламная ссылка", "Выгодные покупки с 2ГИС", "Комментарий к адресу" могут продаваться к адресам, не размещённым на карте

                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, Begin = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Order.AddressAdvertisementNonOnTheMap { OrderId = 1, AddressId = 2, OrderPositionId = 3, PositionId = 4 },
                    new Aggregates::Order.AddressAdvertisementNonOnTheMap { OrderId = 1, AddressId = 3, OrderPositionId = 3, PositionId = 4 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                        new Reference<EntityTypeFirmAddress>(2),
                                        new Reference<EntityTypeOrder>(1),
                                        new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                            new Reference<EntityTypeOrderPosition>(3),
                                            new Reference<EntityTypePosition>(4)))
                                    .ToXDocument(),
                            MessageType = (int)MessageTypeCode.FirmAddressMustBeLocatedOnTheMap,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams =
                                new MessageParams(
                                        new Reference<EntityTypeFirmAddress>(3),
                                        new Reference<EntityTypeOrder>(1),
                                        new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                            new Reference<EntityTypeOrderPosition>(3),
                                            new Reference<EntityTypePosition>(4)))
                                    .ToXDocument(),
                        MessageType = (int)MessageTypeCode.FirmAddressMustBeLocatedOnTheMap,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(2),
                        OrderId = 1,
                    });
    }
}
