using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
    using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CategoryGroupAggregate
            => ArrangeMetadataElement.Config
                                     .Name(nameof(CategoryGroupAggregate))
                                     .CustomerIntelligence(new CI::CategoryGroup { Id = 1, Name = "Name", Rate = 0.999f })
                                     .Fact(new Facts::CategoryGroup { Id = 1, Name = "Name", Rate = 0.999f });
    }
}
