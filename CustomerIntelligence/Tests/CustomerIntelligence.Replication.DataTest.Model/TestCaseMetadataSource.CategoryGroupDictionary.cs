using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
    using Erm = NuClear.CustomerIntelligence.Domain.Model.Erm;
    using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CategoryGroupDictionary
            => ArrangeMetadataElement.Config
                .Name(nameof(CategoryGroupDictionary))
                .CustomerIntelligence(
                    new CI::CategoryGroup { Id = 1, Name = "Первая", Rate = 1.2f },
                    new CI::CategoryGroup { Id = 3, Name = "Третья", Rate = 1.0f },
                    new CI::CategoryGroup { Id = 5, Name = "Пятая", Rate = 0.8f })
                .Fact(
                    new Facts::CategoryGroup { Id = 1, Name = "Первая", Rate = 1.2f },
                    new Facts::CategoryGroup { Id = 3, Name = "Третья", Rate = 1.0f },
                    new Facts::CategoryGroup { Id = 5, Name = "Пятая", Rate = 0.8f })
                .Erm(
                    new Erm::CategoryGroup { Id = 1, Name = "Первая", Rate = 1.2m, IsActive = true, IsDeleted = false },
                    new Erm::CategoryGroup { Id = 3, Name = "Третья", Rate = 1.0m, IsActive = true, IsDeleted = false },
                    new Erm::CategoryGroup { Id = 5, Name = "Пятая", Rate = 0.8m, IsActive = true, IsDeleted = false });
    }
}
