using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Specifications
{
    using Bit = NuClear.CustomerIntelligence.Domain.Model.Bit;

    public static partial class Specs
    {
        public static partial class Map
        {
            public static partial class Bit
            {
                public static IMapSpecification<FirmStatisticsDto, IReadOnlyCollection<Bit::FirmCategoryStatistics>> FirmCategoryStatistics()
                {
                    return new MapSpecification<FirmStatisticsDto, IReadOnlyCollection<Bit::FirmCategoryStatistics>>(
                        dto =>
                        {
                            return dto.Firms
                                        .SelectMany(x => x.Categories.Select(y => new Bit::FirmCategoryStatistics
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

                public static IMapSpecification<CategoryStatisticsDto, IReadOnlyCollection<Bit::ProjectCategoryStatistics>> ProjectCategoryStatistics()
                {
                    return new MapSpecification<CategoryStatisticsDto, IReadOnlyCollection<Bit::ProjectCategoryStatistics>>(
                        dto =>
                            {
                                return dto.Categories
                                          .Select(x => new Bit::ProjectCategoryStatistics
                                              {
                                                  ProjectId = dto.ProjectId,
                                                  CategoryId = x.CategoryId,
                                                  AdvertisersCount = x.AdvertisersCount
                                              })
                                          .ToArray();
                            });
                }

                public static IMapSpecification<FirmForecastDto, IReadOnlyCollection<Bit::FirmCategoryForecast>> FirmCategoryForecasts()
                {
                    return new MapSpecification<FirmForecastDto, IReadOnlyCollection<Bit::FirmCategoryForecast>>(
                        dto =>
                            {
                                var forecasts = from firm in dto.Firms
                                                from category in firm.Categories
                                                select new Bit::FirmCategoryForecast
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

                public static IMapSpecification<FirmForecastDto, IReadOnlyCollection<Bit::FirmForecast>> FirmForecasts()
                {
                    return new MapSpecification<FirmForecastDto, IReadOnlyCollection<Bit::FirmForecast>>(
                        dto =>
                        {
                            var forecasts = from firm in dto.Firms
                                            select new Bit::FirmForecast
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