using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Specifications
{
    using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;

    public static partial class Specs
    {
        public static partial class Map
        {
            public static class CI
            {
                public static class ToStatistics
                {
                    public static readonly MapSpecification<IQuery, IQueryable<Statistics::FirmCategory3>> FirmCategory3 =
                        new MapSpecification<IQuery, IQueryable<Statistics::FirmCategory3>>(q => q.For<Statistics::FirmCategory3>());

                    public static readonly MapSpecification<IQuery, IQueryable<Statistics::FirmForecast>> FirmForecast =
                        new MapSpecification<IQuery, IQueryable<Statistics::FirmForecast>>(q => q.For<Statistics::FirmForecast>());
                }
            }
        }
    }
}