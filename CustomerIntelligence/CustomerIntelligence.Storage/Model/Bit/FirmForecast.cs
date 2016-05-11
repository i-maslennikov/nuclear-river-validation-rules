namespace NuClear.CustomerIntelligence.Storage.Model.Bit
{
    public sealed class FirmForecast
    {
        public long FirmId { get; set; }
        public long ProjectId { get; set; }
        public int ForecastClick { get; set; }
        public decimal ForecastAmount { get; set; }
    }
}