namespace NuClear.CustomerIntelligence.Domain.Model.Bit
{
    public sealed class ProjectCategoryStatistics
    {
        public long ProjectId { get; set; }

        public long CategoryId { get; set; }

        public long AdvertisersCount { get; set; }
    }
}