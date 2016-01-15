namespace NuClear.CustomerIntelligence.Domain.Model.Bit
{
    public sealed class FirmCategoryStatistics : IBitFactObject
    {
        public long FirmId { get; set; }

        public long ProjectId { get; set; }

        public long CategoryId { get; set; }

        public int Hits { get; set; }

        public int Shows { get; set; }
    }
}