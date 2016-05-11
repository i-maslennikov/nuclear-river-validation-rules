namespace NuClear.CustomerIntelligence.Storage.Model.Statistics
{
    public sealed class FirmForecast
    {
        public long ProjectId { get; set; }

        public long FirmId { get; set; }

        public int ForecastClick { get; set; }

        public decimal ForecastAmount { get; set; }
    }
}