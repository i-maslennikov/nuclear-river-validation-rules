using System;

using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    using Facts = Domain.Model.Facts;
    using Aggs = Domain.Model.Aggregates;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement DeniedPositionTests
        => ArrangeMetadataElement.Config
        .Name(nameof(DeniedPositionTests))
        .Fact(
            // denied
            new Facts::DeniedPosition { PositionId = 1, PositionDeniedId = 2, ObjectBindingType = 3, PriceId = 4, Id = 1},
            // global denied
            new Facts::GlobalDeniedPosition { MasterPositionId = 2, DeniedPositionId = 3, ObjectBindingType = 4 },
            // denied & global denied
            new Facts::DeniedPosition { PositionId = 3, PositionDeniedId = 4, ObjectBindingType = 5, PriceId = 6, Id = 2 },
            new Facts::GlobalDeniedPosition { MasterPositionId = 3, DeniedPositionId = 4, ObjectBindingType = 5 }
            )
        .Aggregate(
            // denied
            new Aggs::DeniedPosition { PositionId = 1, DeniedPositionId = 2, ObjectBindingType = 3, PriceId = 4},
            // global denied
            new Aggs::DeniedPosition { PositionId = 2, DeniedPositionId = 3, ObjectBindingType = 4 },
            // denied & global denied
            new Aggs::DeniedPosition { PositionId = 3, DeniedPositionId = 4, ObjectBindingType = 5, PriceId = 6 },
            new Aggs::DeniedPosition { PositionId = 3, DeniedPositionId = 4, ObjectBindingType = 5 }
            );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement MasterPositionTests
        => ArrangeMetadataElement.Config
        .Name(nameof(MasterPositionTests))
        .Fact(
            // associated
            new Facts::AssociatedPosition { PositionId = 1, ObjectBindingType = 3, AssociatedPositionsGroupId = 1, Id = 1 },
            new Facts::AssociatedPositionsGroup { Id = 1, PricePositionId = 1},
            new Facts::PricePosition { Id = 1, PositionId = 2, PriceId = 1 },

            // global associated
            new Facts::GlobalAssociatedPosition { MasterPositionId = 2, AssociatedPositionId = 1, ObjectBindingType = 3 },

            // associated & global associated
            new Facts::AssociatedPosition { PositionId = 3, ObjectBindingType = 4, AssociatedPositionsGroupId = 2, Id = 2},
            new Facts::AssociatedPositionsGroup { Id = 2, PricePositionId = 2 },
            new Facts::PricePosition { Id = 2, PositionId = 4, PriceId = 1 },
            new Facts::GlobalAssociatedPosition { MasterPositionId = 3, AssociatedPositionId = 5, ObjectBindingType = 5 },

            new Facts::Price { Id = 1 }
            )
        .Aggregate(
            // associated
            new Aggs::MasterPosition { MasterPositionId = 1 , PositionId = 2, ObjectBindingType = 3, PriceId = 1, GroupId = 1},
            new Aggs::AdvertisementAmountRestriction { PositionId = 2, PriceId = 1},

            // global associated
            new Aggs::MasterPosition { MasterPositionId = 2, PositionId = 1, ObjectBindingType = 3 },

            // associated & global associated
            new Aggs::MasterPosition { MasterPositionId = 3, PositionId = 4, ObjectBindingType = 4, PriceId = 1, GroupId = 2 },
            new Aggs::MasterPosition { MasterPositionId = 3, PositionId = 5, ObjectBindingType = 5 },
            new Aggs::AdvertisementAmountRestriction { PositionId = 4, PriceId = 1 },

            new Aggs::Price { Id = 1 },
            new Aggs::Period { End = DateTime.MaxValue }
            );
    }
}
