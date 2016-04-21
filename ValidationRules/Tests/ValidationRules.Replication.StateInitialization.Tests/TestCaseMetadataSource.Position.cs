using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    using Facts = Domain.Model.Facts;
    using Aggs = Domain.Model.Aggregates;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement Position
        => ArrangeMetadataElement.Config
        .Name(nameof(Position))
        .Fact(
            new Facts::Position { Id = 1, CategoryCode = 2, IsControlledByAmount = true, Name = "1" }
            )
        .Aggregate(
            new Aggs::Position { Id = 1, CategoryCode = 2, IsControlledByAmount = true, Name = "1" }
            );
    }
}
