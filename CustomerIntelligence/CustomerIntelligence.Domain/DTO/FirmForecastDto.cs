using System.Collections.Generic;

using NuClear.CustomerIntelligence.Domain.Model;

namespace NuClear.CustomerIntelligence.Domain.DTO
{
    public sealed class FirmForecastDto : IBitDto
    {
        public long ProjectId { get; set; }
        public IReadOnlyCollection<FirmDto> Firms { get; set; }

        public sealed class FirmDto
        {
            public long Id { get; set; }
            public int ForecastClick { get; set; }
            public decimal ForecastAmount { get; set; }
            public IReadOnlyCollection<CategoryDto> Categories { get; set; }
        }

        public sealed class CategoryDto
        {
            public long Id { get; set; }
            public int ForecastClick { get; set; }
            public decimal ForecastAmount { get; set; }
        }
    }
}