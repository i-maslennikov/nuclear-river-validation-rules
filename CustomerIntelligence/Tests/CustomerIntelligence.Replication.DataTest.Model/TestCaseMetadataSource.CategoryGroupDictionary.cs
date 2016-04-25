using NuClear.CustomerIntelligence.Storage.Model.CI;
using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CategoryGroupDictionary
            => ArrangeMetadataElement.Config
                .Name(nameof(CategoryGroupDictionary))
                .CustomerIntelligence(
                    new CategoryGroup { Id = 1, Name = "Первая", Rate = 1.2f },
                    new CategoryGroup { Id = 3, Name = "Третья", Rate = 1.0f },
                    new CategoryGroup { Id = 5, Name = "Пятая", Rate = 0.8f })
                .Fact(
                    new Storage.Model.Facts.CategoryGroup { Id = 1, Name = "Первая", Rate = 1.2f },
                    new Storage.Model.Facts.CategoryGroup { Id = 3, Name = "Третья", Rate = 1.0f },
                    new Storage.Model.Facts.CategoryGroup { Id = 5, Name = "Пятая", Rate = 0.8f })
                .Erm(
                    new Storage.Model.Erm.CategoryGroup { Id = 1, Name = "Первая", Rate = 1.2m, IsActive = true, IsDeleted = false },
                    new Storage.Model.Erm.CategoryGroup { Id = 3, Name = "Третья", Rate = 1.0m, IsActive = true, IsDeleted = false },
                    new Storage.Model.Erm.CategoryGroup { Id = 5, Name = "Пятая", Rate = 0.8m, IsActive = true, IsDeleted = false });
    }
}
