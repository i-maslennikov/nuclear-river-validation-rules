using NuClear.CustomerIntelligence.Storage.Model.Common;

namespace NuClear.CustomerIntelligence.Storage.Model.CI
{
    public sealed class FirmLead
    {
        public long FirmId { get; set; }
        public long LeadId { get; set; }
        public bool IsInQueue { get; set; }
        public LeadType Type { get; set; }
    }
}