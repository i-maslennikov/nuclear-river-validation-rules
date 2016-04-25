using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.DTO;
using NuClear.CustomerIntelligence.Storage.Model.Bit;
using NuClear.Storage.API.Specifications;

using FirmForecast = NuClear.CustomerIntelligence.Replication.DTO.FirmForecast;

namespace NuClear.CustomerIntelligence.Replication.Specifications
{
    public static partial class Specs
    {
        public static partial class Map
        {
            public static partial class Bit
            {
                public static MapSpecification<FirmPopularity, IReadOnlyCollection<FirmCategoryStatistics>> FirmCategoryStatistics()
                {
                    return new MapSpecification<FirmPopularity, IReadOnlyCollection<FirmCategoryStatistics>>(
                        dto =>
                        {
                            return dto.Firms
                                        .SelectMany(x => x.Categories.Select(y => new FirmCategoryStatistics
                                            {
                                                ProjectId = dto.ProjectId,
                                                FirmId = x.FirmId,
                                                CategoryId = y.CategoryId,
                                                Hits = y.Hits,
                                                Shows = y.Shows,
                                            }))
                                        .ToArray();
                        });
                }

                public static MapSpecification<RubricPopularity, IReadOnlyCollection<ProjectCategoryStatistics>> ProjectCategoryStatistics()
                {
                    return new MapSpecification<RubricPopularity, IReadOnlyCollection<ProjectCategoryStatistics>>(
                        dto =>
                            {
                                return dto.Categories
                                          .Select(x => new ProjectCategoryStatistics
                                              {
                                                  ProjectId = dto.ProjectId,
                                                  CategoryId = x.CategoryId,
                                                  AdvertisersCount = x.AdvertisersCount
                                              })
                                          .ToArray();
                            });
                }

                public static MapSpecification<FirmForecast, IReadOnlyCollection<FirmCategoryForecast>> FirmCategoryForecasts()
                {
                    return new MapSpecification<FirmForecast, IReadOnlyCollection<FirmCategoryForecast>>(
                        dto =>
                            {
                                var forecasts = from firm in dto.Firms
                                                from category in firm.Categories
                                                select new FirmCategoryForecast
                                                    {
                                                        ProjectId = dto.ProjectId,
                                                        FirmId = firm.Id,
                                                        CategoryId = category.Id,
                                                        ForecastClick = category.ForecastClick,
                                                        ForecastAmount = category.ForecastAmount,
                                                    };

                                return forecasts.ToArray();
                            });
                }

                public static IMapSpecification<FirmForecast, IReadOnlyCollection<Storage.Model.Bit.FirmForecast>> FirmForecasts()
                {
                    return new MapSpecification<FirmForecast, IReadOnlyCollection<Storage.Model.Bit.FirmForecast>>(
                        dto =>
                        {
                            var forecasts = from firm in dto.Firms
                                            select new Storage.Model.Bit.FirmForecast
                                            {
                                                ProjectId = dto.ProjectId,
                                                FirmId = firm.Id,
                                                ForecastClick = firm.ForecastClick,
                                                ForecastAmount = firm.ForecastAmount,
                                            };

                            return forecasts.ToArray();
                        });
                }
            }
        }
    }
}