using NuClear.River.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Statistics
{
    public sealed class FirmForecast : ICustomerIntelligenceAggregatePart, IAggregateValueObject
    {
        public long ProjectId { get; set; }

        public long FirmId { get; set; }

        public int ForecastClick { get; set; }

        public decimal ForecastAmount { get; set; }
    }
}