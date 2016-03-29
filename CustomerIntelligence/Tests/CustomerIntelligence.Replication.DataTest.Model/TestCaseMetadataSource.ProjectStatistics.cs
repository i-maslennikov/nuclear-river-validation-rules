using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;
    using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;
    using Bit = NuClear.CustomerIntelligence.Domain.Model.Bit;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ProjectStatistics
            => ArrangeMetadataElement.Config
                                     .Name(nameof(ProjectStatistics))
                                     .Bit(
                                        new Bit::ProjectCategoryStatistics {ProjectId = 3, AdvertisersCount = 10, CategoryId = 4 },
                                        new Bit::FirmCategoryStatistics { ProjectId = 3, FirmId = 1, CategoryId = 4, Hits = 500, Shows = 1000 },
                                        new Bit::FirmCategoryForecast { ProjectId = 3, CategoryId = 4, FirmId = 1, ForecastAmount = 999, ForecastClick = 333 },
                                        new Bit::FirmForecast { ProjectId = 3, FirmId = 1, ForecastAmount = 99, ForecastClick = 33})
                                     .Fact(
                                        new Facts::Firm {Id = 1, OrganizationUnitId = 2},
                                        new Facts::Project {Id = 3, OrganizationUnitId = 2},
                                        new Facts::FirmAddress {Id = 1, FirmId = 1},
                                        new Facts::CategoryFirmAddress {Id = 1, FirmAddressId = 1, CategoryId = 4},
                                        new Facts::CategoryFirmAddress {Id = 2, FirmAddressId = 1, CategoryId = 5},
                                        new Facts::Category { Id = 4 },
                                        new Facts::Category { Id = 5 })
                                     .Statistics(
                                        new Statistics::ProjectStatistics { Id = 3 },
                                        new Statistics::ProjectCategoryStatistics { ProjectId = 3, CategoryId = 4},
                                        new Statistics::FirmCategory3 { ProjectId = 3, CategoryId = 4, FirmId = 1, AdvertisersShare = 1, FirmCount = 1, ForecastAmount = 999, ForecastClick = 333, Hits = 500, Shows = 1000 },
                                        new Statistics::FirmCategory3 { ProjectId = 3, CategoryId = 5, FirmId = 1, AdvertisersShare = 0, FirmCount = 1, ForecastAmount = null, ForecastClick = null, Hits = 0, Shows = 0 },
                                        new Statistics::FirmForecast { ProjectId = 3, FirmId = 1, ForecastClick = 33, ForecastAmount = 99 });
    }
}
