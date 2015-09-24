using NuClear.Replication.Metadata.Model;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class FirmCategory : IFactStatisticsObject
    {
        public long ProjectId { get; set; }
        public long FirmId { get; set; }
        public long CategoryId { get; set; }
    }
}