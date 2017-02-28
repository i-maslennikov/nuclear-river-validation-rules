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
                    new Facts::FirmAddress { Id = 2, IsLocatedOnTheMap = false, IsActive = true },
                    new Facts::Position { Id = 4 },
                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, Begin = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Order.AddressAdvertisement { OrderId = 1, AddressId = 2, MustBeLocatedOnTheMap = true, OrderPositionId = 3, PositionId = 4 },
                    new Aggregates::FirmAddress { Id = 2, IsLocatedOnTheMap = false })
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
                            Result = 1023,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmAddressMustBeLocatedOnTheMapFirmAddressNotActive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmAddressMustBeLocatedOnTheMapFirmAddressNotActive))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Facts::OrderPosition { Id = 3, OrderId = 1, PricePositionId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 3, FirmAddressId = 2, PositionId = 4 },
                    // firm address not active
                    new Facts::FirmAddress { Id = 2, IsLocatedOnTheMap = false, IsActive = false },
                    new Facts::Position { Id = 4 },
                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, Begin = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::FirmAddress { Id = 2, IsLocatedOnTheMap = false })
                .Message();

        /// <summary>
        /// Позиции "Рекламная ссылка", "Выгодные покупки с 2ГИС", "Комментарий к адресу" могут продаваться к адресам, не размещённым на карте
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmAddressMustBeLocatedOnTheMapSpecialCategoryCode
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmAddressMustBeLocatedOnTheMapSpecialCategoryCode))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Facts::OrderPosition { Id = 3, OrderId = 1, PricePositionId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 3, FirmAddressId = 2, PositionId = 4 },
                    new Facts::FirmAddress { Id = 2, IsLocatedOnTheMap = false, IsActive = true },
                    new Facts::Position { Id = 4, CategoryCode = 11 },
                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, Begin = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Order.AddressAdvertisement { OrderId = 1, AddressId = 2, MustBeLocatedOnTheMap = false, OrderPositionId = 3, PositionId = 4},
                    new Aggregates::FirmAddress { Id = 2, IsLocatedOnTheMap = false })
                .Message();
    }
}
