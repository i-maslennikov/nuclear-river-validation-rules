namespace NuClear.CustomerIntelligence.Storage.Model.Bit
{
    public sealed class FirmCategoryStatistics
    {
        public long FirmId { get; set; }

        public long ProjectId { get; set; }

        public long CategoryId { get; set; }

        public int Hits { get; set; }

        public int Shows { get; set; }
    }
}