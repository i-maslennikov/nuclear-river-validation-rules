using NuClear.CustomerIntelligence.Storage.Model.Common;

namespace NuClear.CustomerIntelligence.Storage.Model.Facts
{
    public sealed class Lead
    {
        public long Id { get; set; }
        public long FirmId { get; set; }
        public long OwnerId { get; set; }
        public LeadType Type { get; set; }
    }
}