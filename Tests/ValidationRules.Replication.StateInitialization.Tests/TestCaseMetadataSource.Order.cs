using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement SimpleOrderPositionTest
            => ArrangeMetadataElement.Config
            .Name(nameof(SimpleOrderPositionTest))
            .Fact(
                new Facts::Order { },
                new Facts::OrderPosition { Id = 1, PricePositionId = 11 },
                new Facts::PricePosition { Id = 11, PositionId = 21, IsActiveNotDeleted = true },
                new Facts::OrderPositionAdvertisement { Id = 31, PositionId = 21, OrderPositionId = 1 },
                new Facts::OrderPositionAdvertisement { Id = 32, PositionId = 21, OrderPositionId = 1, CategoryId = 3 },
                new Facts::OrderPositionAdvertisement { Id = 33, PositionId = 21, OrderPositionId = 1, CategoryId = 1 },
                new Facts::OrderPositionAdvertisement { Id = 34, PositionId = 21, OrderPositionId = 1, CategoryId = 3, FirmAddressId = 111 },
                new Facts::Position { Id = 21, IsComposite = false },
                new Facts::Category { Id = 3, L1Id = 1, L2Id = 2, L3Id = 3 },
                new Facts::Category { Id = 2, L1Id = 1, L2Id = 2 },
                new Facts::Category { Id = 1, L1Id = 1 })
            .Aggregate(
                // OrderPositionAdvertisement
                new Aggregates::Order { },

                new Aggregates::Order.OrderPosition { ItemPositionId = 21 },
                new Aggregates::Order.OrderPosition { ItemPositionId = 21 },
                new Aggregates::Order.OrderPosition { ItemPositionId = 21 },
                new Aggregates::Order.OrderPosition { ItemPositionId = 21 },

                new Aggregates::Position { Id = 21 },
                new Aggregates::Order.OrderPricePosition { OrderPositionId = 1, PositionId = 21, IsActive = true });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PackageOrderPositionTest
            => ArrangeMetadataElement.Config
            .Name(nameof(PackageOrderPositionTest))
            .Fact(
                new Facts::Order { },
                new Facts::OrderPosition { Id = 1, PricePositionId = 11 },
                new Facts::PricePosition { Id = 11, PositionId = 21, IsActiveNotDeleted = true },
                new Facts::OrderPositionAdvertisement { Id = 31, PositionId = 22, OrderPositionId = 1 },
                new Facts::OrderPositionAdvertisement { Id = 32, PositionId = 23, OrderPositionId = 1, CategoryId = 3 },
                new Facts::OrderPositionAdvertisement { Id = 33, PositionId = 24, OrderPositionId = 1, CategoryId = 3 },
                new Facts::OrderPositionAdvertisement { Id = 34, PositionId = 25, OrderPositionId = 1, CategoryId = 3, FirmAddressId = 111 },
                new Facts::Position { Id = 21, IsComposite = true },
                new Facts::Position { Id = 22, IsComposite = false },
                new Facts::Position { Id = 23, IsComposite = false },
                new Facts::Position { Id = 24, IsComposite = false },
                new Facts::Position { Id = 25, IsComposite = false },
                new Facts::Category { Id = 3, L1Id = 1, L2Id = 2, L3Id = 3 },
                new Facts::Category { Id = 2, L1Id = 1, L2Id = 2 },
                new Facts::Category { Id = 1, L1Id = 1 })
            .Aggregate(
                // OrderPositionAdvertisement
                new Aggregates::Order { },

                new Aggregates::Order.OrderPosition { ItemPositionId = 21 },
                new Aggregates::Order.OrderPosition { ItemPositionId = 21 },
                new Aggregates::Order.OrderPosition { ItemPositionId = 21 },

                new Aggregates::Order.OrderPosition { ItemPositionId = 22 },
                new Aggregates::Order.OrderPosition { ItemPositionId = 23 },
                new Aggregates::Order.OrderPosition { ItemPositionId = 24 },
                new Aggregates::Order.OrderPosition { ItemPositionId = 25 },

                new Aggregates::Position { Id = 21 },
                new Aggregates::Position { Id = 22 },
                new Aggregates::Position { Id = 23 },
                new Aggregates::Position { Id = 24 },
                new Aggregates::Position { Id = 25 },
                new Aggregates::Order.OrderPricePosition { OrderPositionId = 1, PositionId = 21, IsActive = true });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPriceTest
            => ArrangeMetadataElement.Config
            .Name(nameof(OrderPriceTest))
            .Fact(
                // 1 order, 1 price position
                new Facts::Order { Id = 1 },
                new Facts::OrderPosition { Id = 10, OrderId = 1, PricePositionId = 10 },
                new Facts::PricePosition { Id = 10, PriceId = 2, PositionId = 10, IsActiveNotDeleted = true },
                new Facts::Position {Id = 10 },

                // 1 order, 2 price positions
                new Facts::Order { Id = 2 },
                new Facts::OrderPosition { Id = 20, OrderId = 2, PricePositionId = 20 },
                new Facts::OrderPosition { Id = 21, OrderId = 2, PricePositionId = 21 },
                new Facts::PricePosition { Id = 20, PriceId = 3, PositionId = 20, IsActiveNotDeleted = true },
                new Facts::PricePosition { Id = 21, PriceId = 3, PositionId = 20, IsActiveNotDeleted = true },
                new Facts::Position {Id = 20, }
                )
            .Aggregate(
                // 1 order, 1 price position
                new Aggregates::Order.OrderPricePosition { OrderId = 1, OrderPositionId = 10, PositionId = 10, PriceId = 2, IsActive = true },
                new Aggregates::Order { Id = 1 },

                // 1 order, 2 price positions
                new Aggregates::Order.OrderPricePosition { OrderId = 2, OrderPositionId = 20, PositionId = 20, PriceId = 3, IsActive = true },
                new Aggregates::Order.OrderPricePosition { OrderId = 2, OrderPositionId = 21, PositionId = 20, PriceId = 3, IsActive = true },
                new Aggregates::Order { Id = 2 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderWithAmountControlledPosition
            => ArrangeMetadataElement.Config
                .Name(nameof(OrderWithAmountControlledPosition))
                .Fact(
                    new Facts::Order { Id = 1 },
                    new Facts::OrderPosition { Id = 1, OrderId = 1, PricePositionId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 1, PositionId = 1 },
                    new Facts::PricePosition { Id = 1, PriceId = 1, PositionId = 1, MinAdvertisementAmount = 1, MaxAdvertisementAmount = 10, IsActiveNotDeleted = true },
                    new Facts::Position { Id = 1, IsControlledByAmount = true, CategoryCode = 10 })
                .Aggregate(
                    new Aggregates::Order { Id = 1 },
                    new Aggregates::Order.AmountControlledPosition { OrderId = 1, CategoryCode = 10 },
                    new Aggregates::Order.OrderPricePosition { OrderId = 1, OrderPositionId = 1, PositionId = 1, PriceId = 1, IsActive = true },
                    new Aggregates::Order.OrderPosition { OrderId = 1, ItemPositionId = 1 },

                    new Aggregates::Position { Id = 1, CategoryCode = 10 },
                    new Aggregates::Price.AdvertisementAmountRestriction { PriceId = 1, CategoryCode = 10, Min = 1, Max = 10, CategoryName = "empty" });
    }
}
