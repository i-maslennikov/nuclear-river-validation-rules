namespace NuClear.CustomerIntelligence.Storage.Model.Facts
{
    public sealed class Lead
    {
        public long Id { get; set; }
        public long FirmId { get; set; }
        public bool IsInQueue { get; set; }
        public int Type { get; set; }
    }
}