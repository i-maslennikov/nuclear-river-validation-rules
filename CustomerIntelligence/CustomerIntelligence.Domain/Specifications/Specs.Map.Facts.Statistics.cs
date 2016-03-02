using System;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Specifications
{
    using Bit = NuClear.CustomerIntelligence.Domain.Model.Bit;
    using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;
    using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;

    public static partial class Specs
    {
        public static partial class Map
        {
            public static partial class Facts
            {
                public static partial class ToStatistics
                {
                    public static readonly MapSpecification<IQuery, IQueryable<Statistics::FirmCategory3>> FirmCategory3 =
                        new MapSpecification<IQuery, IQueryable<Statistics::FirmCategory3>>(
                            q =>
                                {
                                    var firmDtos = from firm in q.For<Facts::Firm>()
                                                    join project in q.For<Facts::Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                                                    join firmAddress in q.For<Facts::FirmAddress>() on firm.Id equals firmAddress.FirmId
                                                    join categoryFirmAddress in q.For<Facts::CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                                                    select new
                                                    {
                                                        FirmId = firm.Id,
                                                        ProjectId = project.Id,
                                                        categoryFirmAddress.CategoryId
                                                    };

                                    var firmCounts = from firm in firmDtos
                                                     group firm by new { firm.ProjectId, firm.CategoryId }
                                                     into grp
                                                     select new
                                                     {
                                                         grp.Key.ProjectId,
                                                         grp.Key.CategoryId,
                                                         Count = grp.Count()
                                                     };

                                    var categories3 = from firmDto in firmDtos.Distinct()
                                                      join firmCount in firmCounts on new { firmDto.ProjectId, firmDto.CategoryId } equals new { firmCount.ProjectId, firmCount.CategoryId }
                                                      join category in q.For<Facts::Category>() on firmDto.CategoryId equals category.Id
                                                      from firmStatistics in q.For<Bit::FirmCategoryStatistics>()
                                                                                  .Where(x => x.FirmId == firmDto.FirmId && x.CategoryId == firmDto.CategoryId && x.ProjectId == firmDto.ProjectId)
                                                                                  .DefaultIfEmpty()
                                                      from categoryStatistics in q.For<Bit::ProjectCategoryStatistics>()
                                                                                  .Where(x => x.CategoryId == firmDto.CategoryId && x.ProjectId == firmDto.ProjectId)
                                                                                  .DefaultIfEmpty()
                                                      from forecast in q.For<Bit::FirmCategoryForecast>()
                                                                                  .Where(x => x.CategoryId == firmDto.CategoryId && x.ProjectId == firmDto.ProjectId)
                                                                                  .DefaultIfEmpty()
                                                      select new Statistics::FirmCategory3
                                                      {
                                                          ProjectId = firmDto.ProjectId,
                                                          FirmId = firmDto.FirmId,
                                                          CategoryId = firmDto.CategoryId,
                                                          Name = category.Name,
                                                          Hits = firmStatistics == null ? 0 : firmStatistics.Hits,
                                                          Shows = firmStatistics == null ? 0 : firmStatistics.Shows,
                                                          FirmCount = firmCount.Count,
                                                          AdvertisersShare = categoryStatistics == null ? 0 : Math.Min(1, (float)categoryStatistics.AdvertisersCount / firmCount.Count),
                                                          ForecastClick = forecast.ForecastClick,
                                                          ForecastAmount = forecast.ForecastAmount
                                                      };

                                    return categories3;
                                });

                    public static readonly MapSpecification<IQuery, IQueryable<Statistics::FirmForecast>> FirmForecast =
                        new MapSpecification<IQuery, IQueryable<Statistics::FirmForecast>>(
                            q =>
                                {
                                    var firmDtos = from firm in q.For<Facts::Firm>()
                                                   join project in q.For<Facts::Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                                                   from forecast in q.For<Bit::FirmForecast>()
                                                                     .Where(x => x.FirmId == firm.Id)
                                                                     .DefaultIfEmpty()
                                                   select new Statistics::FirmForecast
                                                       {
                                                           ProjectId = project.Id,
                                                           FirmId = firm.Id,
                                                           ForecastClick = forecast.ForecastClick,
                                                           ForecastAmount = forecast.ForecastAmount
                                                       };

                                    return firmDtos;
                                });
                }
            }
        }
    }
}