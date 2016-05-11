using System;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Model.Aggregates;
using NuClear.ValidationRules.Storage.Model.Facts;

using Order = NuClear.ValidationRules.Storage.Model.Facts.Order;
using OrderPosition = NuClear.ValidationRules.Storage.Model.Facts.OrderPosition;
using Position = NuClear.ValidationRules.Storage.Model.Facts.Position;

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
            new OrderPosition { Id = 1, PricePositionId = 1 },
            new PricePosition { Id = 1, PositionId = 2 },
            new OrderPositionAdvertisement {Id = 1, PositionId = 3, CategoryId = 10, FirmAddressId = 11, OrderPositionId = 1 },

            // OrderPosition
            new OrderPosition { PricePositionId = 2 },
            new PricePosition { Id = 2, PositionId = 3 },
            new Position { Id = 3, IsComposite = true },

            // OrderPositionAdvertisement & OrderPosition
            new OrderPosition { Id = 3, PricePositionId = 3 },
            new PricePosition { Id = 3, PositionId = 4 },
            new OrderPositionAdvertisement {Id = 3, PositionId = 5, CategoryId = 10, FirmAddressId = 11, OrderPositionId = 3 },
            new Position { Id = 4, IsComposite = true },

            // OrderPositionAdvertisement & Category1
            new OrderPosition { Id = 4, PricePositionId = 4 },
            new PricePosition { Id = 4 },
            new OrderPositionAdvertisement { Id = 4, CategoryId = 3, OrderPositionId = 4 },
            new Category { Id = 3, ParentId = 2 },
            new Category { Id = 2, ParentId = 1 },
            new Category { Id = 1 }

            )
        .Aggregate(
            // OrderPositionAdvertisement
            new Storage.Model.Aggregates.OrderPosition { PackagePositionId = 2, ItemPositionId = 3, Category3Id = 10, FirmAddressId = 11 },
            new AdvertisementAmountRestriction { PositionId = 2 },

            // OrderPosition
            new Storage.Model.Aggregates.OrderPosition { PackagePositionId = 3, ItemPositionId = 3 },
            new AdvertisementAmountRestriction { PositionId = 3 },
            new Storage.Model.Aggregates.Position { Id = 3 },

            // OrderPositionAdvertisement & OrderPosition
            new Storage.Model.Aggregates.OrderPosition { PackagePositionId = 4, ItemPositionId = 5, Category3Id = 10, FirmAddressId = 11 },
            new Storage.Model.Aggregates.OrderPosition { PackagePositionId = 4, ItemPositionId = 4 },
            new AdvertisementAmountRestriction { PositionId = 4 },
            new Storage.Model.Aggregates.Position { Id = 4 },

            // OrderPositionAdvertisement & Category1
            new Storage.Model.Aggregates.OrderPosition { Category3Id = 3, Category1Id = 1 },
            new AdvertisementAmountRestriction()
            );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPriceTest
            => ArrangeMetadataElement.Config
                                     .Name(nameof(OrderPriceTest))
                                     .Fact(
                                           // 1 order, 1 price position
                                           new Order { Id = 1 },
                                           new OrderPosition { Id = 10, OrderId = 1, PricePositionId = 10 },
                                           new PricePosition { Id = 10, PriceId = 2 },

                                           // 1 order, 2 price positions
                                           new Order { Id = 2 },
                                           new OrderPosition { Id = 20, OrderId = 2, PricePositionId = 20 },
                                           new OrderPosition { Id = 21, OrderId = 2, PricePositionId = 21 },
                                           new PricePosition { Id = 20, PriceId = 3, PositionId = 1 },
                                           new PricePosition { Id = 21, PriceId = 3, PositionId = 2 })
                                     .Aggregate(
                                                new Period { Start = DateTime.MinValue, End = DateTime.MaxValue },

                                                // 1 order, 1 price position
                                                new OrderPrice { OrderId = 1, PriceId = 2 },
                                                new AdvertisementAmountRestriction { PriceId = 2 },
                                                new Storage.Model.Aggregates.Order { Id = 1 },

                                                // 1 order, 2 price positions
                                                new OrderPrice { OrderId = 2, PriceId = 3 },
                                                new AdvertisementAmountRestriction { PriceId = 3, PositionId = 1 },
                                                new AdvertisementAmountRestriction { PriceId = 3, PositionId = 2 },
                                                new Storage.Model.Aggregates.Order { Id = 2 });
    }
}
