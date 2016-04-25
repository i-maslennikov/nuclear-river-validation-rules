using NuClear.CustomerIntelligence.Storage.Model.Bit;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.CustomerIntelligence.Storage.Model.Statistics;
using NuClear.DataTest.Metamodel.Dsl;

using FirmForecast = NuClear.CustomerIntelligence.Storage.Model.Bit.FirmForecast;
using ProjectCategoryStatistics = NuClear.CustomerIntelligence.Storage.Model.Bit.ProjectCategoryStatistics;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ProjectStatistics
            => ArrangeMetadataElement.Config
                .Name(nameof(ProjectStatistics))
                .Bit(
                    new ProjectCategoryStatistics {ProjectId = 3, AdvertisersCount = 10, CategoryId = 4 },
                    new FirmCategoryStatistics { ProjectId = 3, FirmId = 1, CategoryId = 4, Hits = 500, Shows = 1000 },
                    new FirmCategoryForecast { ProjectId = 3, CategoryId = 4, FirmId = 1, ForecastAmount = 999, ForecastClick = 333 },
                    new FirmForecast { ProjectId = 3, FirmId = 1, ForecastAmount = 99, ForecastClick = 33})
                .Fact(
                    new Firm {Id = 1, OrganizationUnitId = 2},
                    new Project {Id = 3, OrganizationUnitId = 2},
                    new CategoryOrganizationUnit { Id = 1, CategoryId = 4, OrganizationUnitId = 2 },
                    new FirmAddress {Id = 1, FirmId = 1},
                    new CategoryFirmAddress {Id = 1, FirmAddressId = 1, CategoryId = 4},
                    new CategoryFirmAddress {Id = 2, FirmAddressId = 1, CategoryId = 5},
                    new Category { Id = 4 },
                    new Category { Id = 5 })
                .Statistics(
                    new ProjectStatistics { Id = 3 },
                    new Storage.Model.Statistics.ProjectCategoryStatistics { ProjectId = 3, CategoryId = 4},
                    new FirmCategory3 { ProjectId = 3, CategoryId = 4, FirmId = 1, AdvertisersShare = 1, FirmCount = 1, ForecastAmount = 999, ForecastClick = 333, Hits = 500, Shows = 1000 },
                    new FirmCategory3 { ProjectId = 3, CategoryId = 5, FirmId = 1, AdvertisersShare = 0, FirmCount = 1, ForecastAmount = null, ForecastClick = null, Hits = 0, Shows = 0 },
                    new Storage.Model.Statistics.FirmForecast { ProjectId = 3, FirmId = 1, ForecastClick = 33, ForecastAmount = 99 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement Statistics
            => ArrangeMetadataElement.Config
                .Name(nameof(Statistics))
                .Fact(
                    new Firm { Id = 1, OrganizationUnitId = 1 },
                    new FirmAddress { Id = 1, FirmId = 1 },
                    new CategoryFirmAddress { Id = 1, CategoryId = 1, FirmAddressId = 1 },
                    new CategoryFirmAddress { Id = 2, CategoryId = 2, FirmAddressId = 1 },
                    new CategoryFirmAddress { Id = 3, CategoryId = 3, FirmAddressId = 1 },
                    new CategoryFirmAddress { Id = 4, CategoryId = 4, FirmAddressId = 1 },
                    new Project { Id = 1, OrganizationUnitId = 1 },
                    new CategoryOrganizationUnit { Id = 1, CategoryId = 1, OrganizationUnitId = 1 },
                    new CategoryOrganizationUnit { Id = 2, CategoryId = 2, OrganizationUnitId = 1 },
                    new CategoryOrganizationUnit { Id = 3, CategoryId = 3, OrganizationUnitId = 1 },
                    new CategoryOrganizationUnit { Id = 4, CategoryId = 4, OrganizationUnitId = 1 },
                    new Category { Id = 1, },
                    new Category { Id = 2, },
                    new Category { Id = 3, },
                    new Category { Id = 4, })
                .Bit(
                    new FirmCategoryStatistics { FirmId = 1, CategoryId = 2, ProjectId = 1, Hits = 10, Shows = 100 },
                    new FirmCategoryStatistics { FirmId = 1, CategoryId = 4, ProjectId = 1, Hits = 10, Shows = 100 },

                    new ProjectCategoryStatistics { ProjectId = 1, CategoryId = 1, AdvertisersCount = 0 },
                    new ProjectCategoryStatistics { ProjectId = 1, CategoryId = 2, AdvertisersCount = 1 },
                    new ProjectCategoryStatistics { ProjectId = 1, CategoryId = 3, AdvertisersCount = 100 },

                    new FirmCategoryForecast { FirmId = 1, CategoryId = 3, ProjectId = 1, ForecastClick = 10, ForecastAmount = 999.9999m },
                    new FirmCategoryForecast { FirmId = 1, CategoryId = 4, ProjectId = 1, ForecastClick = 10, ForecastAmount = 999.9999m },

                    // Прогнозы, привязанные к неизвестным фирмам ни на что не влияют
                    new FirmCategoryForecast { FirmId = 2, CategoryId = 2, ProjectId = 1, ForecastClick = 20, ForecastAmount = 1999.9999m })
                .Statistics(
                    // Сущности-идентификаторы
                    new ProjectStatistics { Id = 1 },
                    new Storage.Model.Statistics.ProjectCategoryStatistics { ProjectId = 1, CategoryId = 1 },
                    new Storage.Model.Statistics.ProjectCategoryStatistics { ProjectId = 1, CategoryId = 2 },
                    new Storage.Model.Statistics.ProjectCategoryStatistics { ProjectId = 1, CategoryId = 3 },
                    new Storage.Model.Statistics.ProjectCategoryStatistics { ProjectId = 1, CategoryId = 4 },

                    // При отсутствии данных, статистика подразумевается нулевой, а прогнозы должны явно указывать на это.
                    // При этом, если у фирмы есть рубрика - то запись должна быть обязательно.
                    new FirmCategory3 { FirmId = 1, CategoryId = 1, ProjectId = 1, Hits = 0, Shows = 0, AdvertisersShare = 0, FirmCount = 1, ForecastClick = null, ForecastAmount = null },
                    // Наличие или отсутствие статистики или прогнозов не должно влиять на второй компонент.
                    new FirmCategory3 { FirmId = 1, CategoryId = 2, ProjectId = 1, Hits = 10, Shows = 100, AdvertisersShare = 1, FirmCount = 1, ForecastClick = null, ForecastAmount = null },
                    new FirmCategory3 { FirmId = 1, CategoryId = 3, ProjectId = 1, Hits = 0, Shows = 0, AdvertisersShare = 1, FirmCount = 1, ForecastClick = 10, ForecastAmount = 999.9999m },
                    new FirmCategory3 { FirmId = 1, CategoryId = 4, ProjectId = 1, Hits = 10, Shows = 100, AdvertisersShare = 0, FirmCount = 1, ForecastClick = 10, ForecastAmount = 999.9999m });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmForecasts
            => ArrangeMetadataElement.Config
                .Name(nameof(FirmForecasts))
                .Fact(
                    new Firm { Id = 1, OrganizationUnitId = 1 },
                    new Firm { Id = 2, OrganizationUnitId = 1 },
                    new Project { Id = 1, OrganizationUnitId = 1 })
                .Bit(
                    new FirmForecast { FirmId = 1, ProjectId = 1, ForecastClick = 10, ForecastAmount = 999.9999m },
                    new FirmForecast { FirmId = 3, ProjectId = 1, ForecastClick = 10, ForecastAmount = 999.9999m })
                .Statistics(
                    new ProjectStatistics { Id = 1 },
                    new Storage.Model.Statistics.FirmForecast { FirmId = 1, ProjectId = 1, ForecastClick = 10, ForecastAmount = 999.9999m });
    }
}
