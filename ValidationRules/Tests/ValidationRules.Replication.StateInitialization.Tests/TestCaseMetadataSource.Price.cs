using System;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // todo: по завршении работ с периодами добавить проверку связи прайса и города
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement Period
        => ArrangeMetadataElement.Config
        .Name(nameof(Period))
        .Fact(
            new Facts::Price { Id = 1, BeginDate = DateTime.Parse("2012-12-12") },
            new Facts::PricePosition { Id = 1, PriceId = 1, PositionId = 2, MinAdvertisementAmount = 100, MaxAdvertisementAmount = 500 },

            // associated
            new Facts::AssociatedPosition { PositionId = 1, ObjectBindingType = 3, AssociatedPositionsGroupId = 1, Id = 1 },
            new Facts::AssociatedPositionsGroup { Id = 1, PricePositionId = 1},

            // denied
            new Facts::DeniedPosition { PositionId = 1, PositionDeniedId = 2, ObjectBindingType = 3, PriceId = 1, Id = 1 }
            )
        .Aggregate(
            new Storage.Model.Aggregates.Price { Id = 1 },
            new Aggregates::AdvertisementAmountRestriction { PositionId = 2, PriceId = 1, Min = 100, Max = 500 },

            // associated
            new Aggregates::PriceAssociatedPosition { PrincipalPositionId = 1 , AssociatedPositionId = 2, ObjectBindingType = 3, PriceId = 1, GroupId = 1 },

            // denied
            new Aggregates::PriceDeniedPosition { PrincipalPositionId = 1, DeniedPositionId = 2, ObjectBindingType = 3, PriceId = 1 },

            // сопутствующий хлам
            new Aggregates::Period { Start = DateTime.Parse("2012-12-12"), End = DateTime.MaxValue },
            new Aggregates::PricePeriod { PriceId = 1, Start = DateTime.Parse("2012-12-12") }
            );
    }
}
