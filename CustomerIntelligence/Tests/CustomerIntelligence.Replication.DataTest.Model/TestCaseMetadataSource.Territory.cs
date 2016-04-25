using NuClear.CustomerIntelligence.Storage.Model.CI;
using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement TerritoryAggregate
            => ArrangeMetadataElement.Config
                                     .Name(nameof(TerritoryAggregate))
                                     .CustomerIntelligence(new Territory { Id = 1, Name = "TerritoryName", ProjectId = 1 },
                                                           new Project { Id = 1, Name = "ProjectName" })
                                     .Fact(new Storage.Model.Facts.Territory { Id = 1, Name = "TerritoryName", OrganizationUnitId = 2 },
                                           new Storage.Model.Facts.Project { Id = 1, Name = "ProjectName", OrganizationUnitId = 2 });
    }
}
