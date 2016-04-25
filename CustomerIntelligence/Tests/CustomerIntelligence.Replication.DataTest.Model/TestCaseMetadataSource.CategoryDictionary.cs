using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CategoryDictionary
            => ArrangeMetadataElement.Config
                .Name(nameof(CategoryDictionary))
                .Fact(
                    new Category { Id = 1, Level = 1, Name = "Category 1 level 1", ParentId = null },
                    new Category { Id = 2, Level = 2, Name = "Category 2 level 2", ParentId = 1 },
                    new Category { Id = 3, Level = 3, Name = "Category 3 level 3", ParentId = 2 },
                    new Category { Id = 4, Level = 3, Name = "Category 4 level 3", ParentId = 2 })
                .Erm(
                    new Storage.Model.Erm.Category { Id = 1, Level = 1, ParentId = null, Name = "Category 1 level 1", IsActive = true, IsDeleted = false },
                    new Storage.Model.Erm.Category { Id = 2, Level = 2, ParentId = 1, Name = "Category 2 level 2", IsActive = true, IsDeleted = false },
                    new Storage.Model.Erm.Category { Id = 3, Level = 3, ParentId = 2, Name = "Category 3 level 3", IsActive = true, IsDeleted = false },
                    new Storage.Model.Erm.Category { Id = 4, Level = 3, ParentId = 2, Name = "Category 4 level 3", IsActive = true, IsDeleted = false });
    }
}
