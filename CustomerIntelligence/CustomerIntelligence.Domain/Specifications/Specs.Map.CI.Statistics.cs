using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Specifications
{
    using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
    using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;

    public static partial class Specs
    {
        public static partial class Map
        {
            public static partial class CI
            {
                public static partial class ToStatistics
                {
                    public static readonly MapSpecification<IQuery, IQueryable<Statistics::FirmCategory3>> FirmCategory3 =
                        new MapSpecification<IQuery, IQueryable<Statistics::FirmCategory3>>(
                            q => from firm in q.For<CI::Firm>()
                                 join statistics in q.For<Statistics::FirmCategory3>() on firm.Id equals statistics.FirmId
                                 select new Statistics::FirmCategory3
                                 {
                                            ProjectId = firm.ProjectId,

                                            FirmId = statistics.FirmId,
                                            CategoryId = statistics.CategoryId,
                                            Name = statistics.Name,
                                            AdvertisersShare = statistics.AdvertisersShare,
                                            FirmCount = statistics.FirmCount,
                                            Hits = statistics.Hits,
                                            Shows = statistics.Shows,
                                        });
                }
            }
        }
    }
}