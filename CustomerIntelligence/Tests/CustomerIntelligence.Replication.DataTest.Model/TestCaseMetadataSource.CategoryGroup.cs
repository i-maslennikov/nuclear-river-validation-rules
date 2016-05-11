using NuClear.CustomerIntelligence.Storage.Model.CI;
using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CategoryGroupAggregate
            => ArrangeMetadataElement.Config
                                     .Name(nameof(CategoryGroupAggregate))
                                     .CustomerIntelligence(new CategoryGroup { Id = 1, Name = "Name", Rate = 0.999f })
                                     .Fact(new Storage.Model.Facts.CategoryGroup { Id = 1, Name = "Name", Rate = 0.999f });
    }
}
