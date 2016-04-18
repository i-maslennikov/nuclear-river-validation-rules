using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
    using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement TerritoryAggregate
            => ArrangeMetadataElement.Config
                                     .Name(nameof(TerritoryAggregate))
                                     .CustomerIntelligence(new CI::Territory { Id = 1, Name = "TerritoryName", ProjectId = 1 },
                                                           new CI::Project { Id = 1, Name = "ProjectName" })
                                     .Fact(new Facts::Territory { Id = 1, Name = "TerritoryName", OrganizationUnitId = 2 },
                                           new Facts::Project { Id = 1, Name = "ProjectName", OrganizationUnitId = 2 });
    }
}
