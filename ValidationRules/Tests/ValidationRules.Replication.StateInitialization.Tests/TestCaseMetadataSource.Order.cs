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
            new Facts::OrderPositionAdvertisement { Id = 1, PositionId = 3, CategoryId = 10, FirmAddressId = 11, OrderPositionId = 1 },

            // OrderPosition
            new Facts::OrderPosition { Id = 2, PricePositionId = 2 },
            new Facts::PricePosition { Id = 2, PositionId = 3 },
            new Facts::Position { Id = 3, IsComposite = true },

            // OrderPositionAdvertisement & OrderPosition
            new Facts::OrderPosition { Id = 3, PricePositionId = 3 },
            new Facts::PricePosition { Id = 3, PositionId = 4 },
            new Facts::OrderPositionAdvertisement { Id = 3, PositionId = 5, CategoryId = 10, FirmAddressId = 11, OrderPositionId = 3 },
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
            new Aggregates::OrderPosition { OrderPositionId = 1, PackagePositionId = 2, ItemPositionId = 3, Category3Id = 10, FirmAddressId = 11 },

            // OrderPosition
            new Aggregates::OrderPosition { OrderPositionId = 2, PackagePositionId = 3, ItemPositionId = 3 },
            new Aggregates::Position { Id = 3 },

            // OrderPositionAdvertisement & OrderPosition
            new Aggregates::OrderPosition { OrderPositionId = 3, PackagePositionId = 4, ItemPositionId = 5, Category3Id = 10, FirmAddressId = 11 },
            new Aggregates::OrderPosition { OrderPositionId = 3, PackagePositionId = 4, ItemPositionId = 4 },
            new Aggregates::Position { Id = 4 },

            // OrderPositionAdvertisement & Category1
            new Aggregates::OrderPosition { OrderPositionId = 4, Category3Id = 3, Category1Id = 1 },

            new Aggregates::OrderPricePosition { OrderPositionId = 1, IsActive = true },
            new Aggregates::OrderPricePosition { OrderPositionId = 2, IsActive = true },
            new Aggregates::OrderPricePosition { OrderPositionId = 3, IsActive = true },
            new Aggregates::OrderPricePosition { OrderPositionId = 4, IsActive = true }
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
                // 1 order, 1 price position
                new Aggregates::OrderPricePosition { OrderId = 1, OrderPositionId = 10, PriceId = 2, IsActive = true },
                new Aggregates::Order { Id = 1 },

                // 1 order, 2 price positions
                new Aggregates::OrderPricePosition { OrderId = 2, OrderPositionId = 20, PriceId = 3, IsActive = true },
                new Aggregates::OrderPricePosition { OrderId = 2, OrderPositionId = 21, PriceId = 3, IsActive = true },
                new Aggregates::Order { Id = 2 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderWithAmountControlledPosition
            => ArrangeMetadataElement.Config
                .Name(nameof(OrderWithAmountControlledPosition))
                .Fact(
                    new Facts::Order { Id = 1 },
                    new Facts::OrderPosition { Id = 1, OrderId = 1, PricePositionId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 1, PositionId = 1 },
                    new Facts::PricePosition { Id = 1, PriceId = 1, PositionId = 1, MinAdvertisementAmount = 1, MaxAdvertisementAmount = 10 },
                    new Facts::Position { Id = 1, IsControlledByAmount = true, CategoryCode = 10 })
                .Aggregate(
                    new Aggregates::Order { Id = 1 },
                    new Aggregates::AmountControlledPosition { OrderId = 1, CategoryCode = 10 },
                    new Aggregates::OrderPricePosition { OrderId = 1, OrderPositionId = 1, PriceId = 1, IsActive = true },
                    new Aggregates::OrderPosition { OrderId = 1, OrderPositionId = 1, PackagePositionId = 1, ItemPositionId = 1 },

                    new Aggregates::Position { Id = 1, CategoryCode = 10, IsControlledByAmount = true },
                    new Aggregates::AdvertisementAmountRestriction { PriceId = 1, CategoryCode = 10, Min = 1, Max = 10 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderWithDeniedPositions
            => ArrangeMetadataElement.Config
                .Name(nameof(OrderWithDeniedPositions))
                .Fact(
                    new Facts::Order { Id = 1 },
                    new Facts::OrderPosition { Id = 2, OrderId = 1, PricePositionId = 4 },
                    new Facts::OrderPositionAdvertisement { Id = 5, OrderPositionId = 2, PositionId = 7 },
                    new Facts::PricePosition { Id = 4, PriceId = 9, PositionId = 7 },
                    new Facts::DeniedPosition { Id = 11, PriceId = 9, PositionId = 7, PositionDeniedId = 14 })
                .Aggregate(
                    new Aggregates::Order { Id = 1 },
                    new Aggregates::OrderPosition { OrderId = 1, OrderPositionId = 2, ItemPositionId = 7, PackagePositionId = 7 },
                    new Aggregates::OrderDeniedPosition { OrderId = 1, ExceptOrderPositionId = 2, ItemPositionId = 14 },
                    new Aggregates::OrderPricePosition {OrderId = 1, OrderPositionId = 2, PriceId = 9, IsActive = true },

                    new Aggregates::PriceDeniedPosition { PriceId = 9, PrincipalPositionId = 7, DeniedPositionId = 14 });
    }
}
