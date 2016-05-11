using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement Position
        => ArrangeMetadataElement.Config
        .Name(nameof(Position))
        .Fact(
            new Position { Id = 1, CategoryCode = 2, IsControlledByAmount = true, Name = "1" }
            )
        .Aggregate(
            new Storage.Model.Aggregates.Position { Id = 1, CategoryCode = 2, IsControlledByAmount = true, Name = "1" }
            );
    }
}
