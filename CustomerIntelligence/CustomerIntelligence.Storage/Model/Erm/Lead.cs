namespace NuClear.CustomerIntelligence.Storage.Model.Erm
{
    public sealed class Lead
    {
        public long Id { get; set; }
        public long? FirmId { get; set; }
        public long OwnerId { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
    }
}