using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
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
                    new Facts::PricePosition { Id = 1, PriceId = 1, PositionId = 1, IsActiveNotDeleted = true },
                    new Facts::Position { Id = 1, IsControlledByAmount = true, CategoryCode = 10 },
                    new Facts::Project { Id = 123 })
                .Aggregate(
                    new Aggregates::Order { Id = 1 },
                    new Aggregates::Order.AmountControlledPosition { OrderId = 1, CategoryCode = 10, ProjectId = 123 },
                    new Aggregates::Order.OrderPricePosition { OrderId = 1, OrderPositionId = 1, PositionId = 1, PriceId = 1, IsActive = true });
    }
}
