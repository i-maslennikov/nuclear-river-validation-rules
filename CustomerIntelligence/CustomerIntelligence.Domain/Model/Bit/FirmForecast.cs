namespace NuClear.CustomerIntelligence.Domain.Model.Bit
{
    public sealed class FirmForecast : IBitFactObject
    {
        public long FirmId { get; set; }
        public long ProjectId { get; set; }
        public int ForecastClick { get; set; }
        public decimal ForecastAmount { get; set; }
    }
}