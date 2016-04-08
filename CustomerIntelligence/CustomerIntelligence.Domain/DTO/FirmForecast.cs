using System.Collections.Generic;

using NuClear.CustomerIntelligence.Domain.Model;

namespace NuClear.CustomerIntelligence.Domain.DTO
{
    public sealed class FirmForecast : IBitDto
    {
        public long ProjectId { get; set; }
        public IReadOnlyCollection<Firm> Firms { get; set; }

        public sealed class Firm
        {
            public long Id { get; set; }
            public int ForecastClick { get; set; }
            public decimal ForecastAmount { get; set; }
            public IReadOnlyCollection<Category> Categories { get; set; }
        }

        public sealed class Category
        {
            public long Id { get; set; }
            public int ForecastClick { get; set; }
            public decimal ForecastAmount { get; set; }
        }
    }
}