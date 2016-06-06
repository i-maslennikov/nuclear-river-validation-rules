using System;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionTest
        => ArrangeMetadataElement.Config
        .Name(nameof(OrderPositionTest))
        .Fact(
            // OrderPositionAdvertisement
            new Facts::OrderPosition { Id = 1, PricePositionId = 1 },
            new Facts::PricePosition { Id = 1, PositionId = 2 },
            new Facts::OrderPositionAdvertisement {Id = 1, PositionId = 3, CategoryId = 10, FirmAddressId = 11, OrderPositionId = 1 },

            // OrderPosition
            new Facts::OrderPosition { PricePositionId = 2 },
            new Facts::PricePosition { Id = 2, PositionId = 3 },
            new Facts::Position { Id = 3, IsComposite = true },

            // OrderPositionAdvertisement & OrderPosition
            new Facts::OrderPosition { Id = 3, PricePositionId = 3 },
            new Facts::PricePosition { Id = 3, PositionId = 4 },
            new Facts::OrderPositionAdvertisement {Id = 3, PositionId = 5, CategoryId = 10, FirmAddressId = 11, OrderPositionId = 3 },
            new Facts::Position { Id = 4, IsComposite = true },

            // OrderPositionAdvertisement & Category1
            new Facts::OrderPosition { Id = 4, PricePositionId = 4 },
            new Facts::PricePosition { Id = 4 },
            new Facts::OrderPositionAdvertisement { Id = 4, CategoryId = 3, OrderPositionId = 4 },
            new Facts::Category { Id = 3, ParentId = 2 },
            new Facts::Category { Id = 2, ParentId = 1 },
            new Facts::Category { Id = 1 }

            )
        .Aggregate(
            // OrderPositionAdvertisement
            new Aggregates::OrderPosition { PackagePositionId = 2, ItemPositionId = 3, Category3Id = 10, FirmAddressId = 11 },
            new Aggregates::AdvertisementAmountRestriction { PositionId = 2 },

            // OrderPosition
            new Aggregates::OrderPosition { PackagePositionId = 3, ItemPositionId = 3 },
            new Aggregates::AdvertisementAmountRestriction { PositionId = 3 },
            new Aggregates::Position { Id = 3 },

            // OrderPositionAdvertisement & OrderPosition
            new Aggregates::OrderPosition { PackagePositionId = 4, ItemPositionId = 5, Category3Id = 10, FirmAddressId = 11 },
            new Aggregates::OrderPosition { PackagePositionId = 4, ItemPositionId = 4 },
            new Aggregates::AdvertisementAmountRestriction { PositionId = 4 },
            new Aggregates::Position { Id = 4 },

            // OrderPositionAdvertisement & Category1
            new Aggregates::OrderPosition { Category3Id = 3, Category1Id = 1 },
            new Aggregates::AdvertisementAmountRestriction()
            );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPriceTest
        => ArrangeMetadataElement.Config
        .Name(nameof(OrderPriceTest))
        .Fact(
            // 1 order, 1 price position
            new Facts::Order { Id = 1 },
            new Facts::OrderPosition { Id = 10, OrderId = 1, PricePositionId = 10 },
            new Facts::PricePosition { Id = 10, PriceId = 2 },

            // 1 order, 2 price positions
            new Facts::Order { Id = 2 },
            new Facts::OrderPosition { Id = 20, OrderId = 2, PricePositionId = 20 },
            new Facts::OrderPosition { Id = 21, OrderId = 2, PricePositionId = 21 },
            new Facts::PricePosition { Id = 20, PriceId = 3, PositionId = 1 },
            new Facts::PricePosition { Id = 21, PriceId = 3, PositionId = 2 })
        .Aggregate(
            new Aggregates::Period { Start = DateTime.MinValue, End = DateTime.MaxValue },

            // 1 order, 1 price position
            new Aggregates::OrderPricePosition { OrderId = 1, OrderPositionId = 10, PriceId = 2 },
            new Aggregates::AdvertisementAmountRestriction { PriceId = 2 },
            new Aggregates::Order { Id = 1 },

            // 1 order, 2 price positions
            new Aggregates::OrderPricePosition { OrderId = 2, OrderPositionId = 20, PriceId = 3 },
            new Aggregates::OrderPricePosition { OrderId = 2, OrderPositionId = 21, PriceId = 3 },
            new Aggregates::AdvertisementAmountRestriction { PriceId = 3, PositionId = 1 },
            new Aggregates::AdvertisementAmountRestriction { PriceId = 3, PositionId = 2 },
            new Aggregates::Order { Id = 2 });
    }
}
