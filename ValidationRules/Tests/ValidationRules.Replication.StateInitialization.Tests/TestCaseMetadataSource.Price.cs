using System;

using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    using Facts = Domain.Model.Facts;
    using Aggs = Domain.Model.Aggregates;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PriceDeniedPositionTests
        => ArrangeMetadataElement.Config
        .Name(nameof(PriceDeniedPositionTests))
        .Fact(
            // denied
            new Facts::DeniedPosition { PositionId = 1, PositionDeniedId = 2, ObjectBindingType = 3, PriceId = 4, Id = 1}
            )
        .Aggregate(
            // denied
            new Aggs::PriceDeniedPosition { PrincipalPositionId = 1, DeniedPositionId = 2, ObjectBindingType = 3, PriceId = 4}
            );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PriceAssociatedPositionTests
        => ArrangeMetadataElement.Config
        .Name(nameof(PriceAssociatedPositionTests))
        .Fact(
            // associated
            new Facts::AssociatedPosition { PositionId = 1, ObjectBindingType = 3, AssociatedPositionsGroupId = 1, Id = 1 },
            new Facts::AssociatedPositionsGroup { Id = 1, PricePositionId = 1},
            new Facts::PricePosition { Id = 1, PositionId = 2, PriceId = 1 },

            new Facts::Price { Id = 1 }
            )
        .Aggregate(
            // associated
            new Aggs::PriceAssociatedPosition { PrincipalPositionId = 1 , AssociatedPositionId = 2, ObjectBindingType = 3, PriceId = 1, GroupId = 1},
            new Aggs::AdvertisementAmountRestriction { PositionId = 2, PriceId = 1},

            new Aggs::Price { Id = 1 },
            new Aggs::Period { End = DateTime.MaxValue }
            );
    }
}
