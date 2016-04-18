using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
    using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ProjectAggregate
            => ArrangeMetadataElement.Config
                                     .Name(nameof(ProjectAggregate))
                                     .CustomerIntelligence(
                                                           new CI::Project { Id = 1, Name = "ProjectOne" },
                                                           new CI::ProjectCategory { ProjectId = 1, CategoryId = 1, Name = "Category 1 level 1", Level = 1, ParentId = null },
                                                           new CI::ProjectCategory { ProjectId = 1, CategoryId = 2, Name = "Category 2 level 2", Level = 2, ParentId = 1 },
                                                           new CI::ProjectCategory { ProjectId = 1, CategoryId = 3, Name = "Category 3 level 3", Level = 3, ParentId = 2 })
                                     .Fact(
                                           new Facts::Project { Id = 1, Name = "ProjectOne", OrganizationUnitId = 1 },
                                           new Facts::Category { Id = 1, Level = 1, Name = "Category 1 level 1", ParentId = null },
                                           new Facts::Category { Id = 2, Level = 2, Name = "Category 2 level 2", ParentId = 1 },
                                           new Facts::Category { Id = 3, Level = 3, Name = "Category 3 level 3", ParentId = 2 },
                                           new Facts::CategoryOrganizationUnit { Id = 1, CategoryId = 1, OrganizationUnitId = 1 },
                                           new Facts::CategoryOrganizationUnit { Id = 2, CategoryId = 2, OrganizationUnitId = 1 },
                                           new Facts::CategoryOrganizationUnit { Id = 3, CategoryId = 3, OrganizationUnitId = 1 });
    }
}
