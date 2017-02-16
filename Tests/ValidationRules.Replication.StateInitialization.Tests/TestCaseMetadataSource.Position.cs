using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement Position
        => ArrangeMetadataElement.Config
        .Name(nameof(Position))
        .Fact(
            new Facts::Position { Id = 1, CategoryCode = 2, IsControlledByAmount = true }
            )
        .Aggregate(
            new Aggregates::Position { Id = 1, CategoryCode = 2 }
            );
    }
}
