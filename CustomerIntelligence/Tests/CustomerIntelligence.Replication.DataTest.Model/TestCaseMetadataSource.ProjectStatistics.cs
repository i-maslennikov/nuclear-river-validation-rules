using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    using Statistics = Storage.Model.Statistics;
    using Facts = Storage.Model.Facts;
    using Bit = Storage.Model.Bit;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ProjectStatistics
            => ArrangeMetadataElement.Config
                .Name(nameof(ProjectStatistics))
                .Bit(
                    new Bit::ProjectCategoryStatistics { ProjectId = 3, AdvertisersCount = 10, CategoryId = 4 },
                    new Bit::FirmCategoryStatistics { ProjectId = 3, FirmId = 1, CategoryId = 4, Hits = 500, Shows = 1000 },
                    new Bit::FirmCategoryForecast { ProjectId = 3, CategoryId = 4, FirmId = 1, ForecastAmount = 999, ForecastClick = 333 },
                    new Bit::FirmForecast { ProjectId = 3, FirmId = 1, ForecastAmount = 99, ForecastClick = 33 })
                .Fact(
                    new Facts::Firm { Id = 1, OrganizationUnitId = 2 },
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },
                    new Facts::CategoryOrganizationUnit { Id = 1, CategoryId = 4, OrganizationUnitId = 2 },
                    new Facts::FirmAddress { Id = 1, FirmId = 1 },
                    new Facts::CategoryFirmAddress { Id = 1, FirmAddressId = 1, CategoryId = 4 },
                    new Facts::CategoryFirmAddress { Id = 2, FirmAddressId = 1, CategoryId = 5 },
                    new Facts::Category { Id = 4 },
                    new Facts::Category { Id = 5 })
                .Statistics(
                    new Statistics::ProjectStatistics { Id = 3 },
                    new Statistics::ProjectCategoryStatistics { ProjectId = 3, CategoryId = 4 },
                    new Statistics::FirmCategory3 { ProjectId = 3, CategoryId = 4, FirmId = 1, AdvertisersShare = 1, FirmCount = 1, ForecastAmount = 999, ForecastClick = 333, Hits = 500, Shows = 1000 },
                    new Statistics::FirmCategory3 { ProjectId = 3, CategoryId = 5, FirmId = 1, AdvertisersShare = 0, FirmCount = 1, ForecastAmount = null, ForecastClick = null, Hits = 0, Shows = 0 },
                    new Statistics::FirmForecast { ProjectId = 3, FirmId = 1, ForecastClick = 33, ForecastAmount = 99 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement Statistics
            => ArrangeMetadataElement.Config
                .Name(nameof(Statistics))
                .Fact(
                    new Facts::Firm { Id = 1, OrganizationUnitId = 1 },
                    new Facts::FirmAddress { Id = 1, FirmId = 1 },
                    new Facts::CategoryFirmAddress { Id = 1, CategoryId = 1, FirmAddressId = 1 },
                    new Facts::CategoryFirmAddress { Id = 2, CategoryId = 2, FirmAddressId = 1 },
                    new Facts::CategoryFirmAddress { Id = 3, CategoryId = 3, FirmAddressId = 1 },
                    new Facts::CategoryFirmAddress { Id = 4, CategoryId = 4, FirmAddressId = 1 },
                    new Facts::Project { Id = 1, OrganizationUnitId = 1 },
                    new Facts::CategoryOrganizationUnit { Id = 1, CategoryId = 1, OrganizationUnitId = 1 },
                    new Facts::CategoryOrganizationUnit { Id = 2, CategoryId = 2, OrganizationUnitId = 1 },
                    new Facts::CategoryOrganizationUnit { Id = 3, CategoryId = 3, OrganizationUnitId = 1 },
                    new Facts::CategoryOrganizationUnit { Id = 4, CategoryId = 4, OrganizationUnitId = 1 },
                    new Facts::Category { Id = 1, },
                    new Facts::Category { Id = 2, },
                    new Facts::Category { Id = 3, },
                    new Facts::Category { Id = 4, })
                .Bit(
                    new Bit::FirmCategoryStatistics { FirmId = 1, CategoryId = 2, ProjectId = 1, Hits = 10, Shows = 100 },
                    new Bit::FirmCategoryStatistics { FirmId = 1, CategoryId = 4, ProjectId = 1, Hits = 10, Shows = 100 },

                    new Bit::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 1, AdvertisersCount = 0 },
                    new Bit::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 2, AdvertisersCount = 1 },
                    new Bit::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 3, AdvertisersCount = 100 },

                    new Bit::FirmCategoryForecast { FirmId = 1, CategoryId = 3, ProjectId = 1, ForecastClick = 10, ForecastAmount = 999.9999m },
                    new Bit::FirmCategoryForecast { FirmId = 1, CategoryId = 4, ProjectId = 1, ForecastClick = 10, ForecastAmount = 999.9999m },

                    // Прогнозы, привязанные к неизвестным фирмам ни на что не влияют
                    new Bit::FirmCategoryForecast { FirmId = 2, CategoryId = 2, ProjectId = 1, ForecastClick = 20, ForecastAmount = 1999.9999m })
                .Statistics(
                    // Сущности-идентификаторы
                    new Statistics::ProjectStatistics { Id = 1 },
                    new Statistics::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 1 },
                    new Statistics::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 2 },
                    new Statistics::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 3 },
                    new Statistics::ProjectCategoryStatistics { ProjectId = 1, CategoryId = 4 },

                    // При отсутствии данных, статистика подразумевается нулевой, а прогнозы должны явно указывать на это.
                    // При этом, если у фирмы есть рубрика - то запись должна быть обязательно.
                    new Statistics::FirmCategory3 { FirmId = 1, CategoryId = 1, ProjectId = 1, Hits = 0, Shows = 0, AdvertisersShare = 0, FirmCount = 1, ForecastClick = null, ForecastAmount = null },
                    // Наличие или отсутствие статистики или прогнозов не должно влиять на второй компонент.
                    new Statistics::FirmCategory3 { FirmId = 1, CategoryId = 2, ProjectId = 1, Hits = 10, Shows = 100, AdvertisersShare = 1, FirmCount = 1, ForecastClick = null, ForecastAmount = null },
                    new Statistics::FirmCategory3 { FirmId = 1, CategoryId = 3, ProjectId = 1, Hits = 0, Shows = 0, AdvertisersShare = 1, FirmCount = 1, ForecastClick = 10, ForecastAmount = 999.9999m },
                    new Statistics::FirmCategory3 { FirmId = 1, CategoryId = 4, ProjectId = 1, Hits = 10, Shows = 100, AdvertisersShare = 0, FirmCount = 1, ForecastClick = 10, ForecastAmount = 999.9999m });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmForecasts
            => ArrangeMetadataElement.Config
                .Name(nameof(FirmForecasts))
                .Fact(
                    new Facts::Firm { Id = 1, OrganizationUnitId = 1 },
                    new Facts::Firm { Id = 2, OrganizationUnitId = 1 },
                    new Facts::Project { Id = 1, OrganizationUnitId = 1 })
                .Bit(
                    new Bit::FirmForecast { FirmId = 1, ProjectId = 1, ForecastClick = 10, ForecastAmount = 999.9999m },
                    new Bit::FirmForecast { FirmId = 3, ProjectId = 1, ForecastClick = 10, ForecastAmount = 999.9999m })
                .Statistics(
                    new Statistics::ProjectStatistics { Id = 1 },
                    new Statistics::FirmForecast { FirmId = 1, ProjectId = 1, ForecastClick = 10, ForecastAmount = 999.9999m });
    }
}
