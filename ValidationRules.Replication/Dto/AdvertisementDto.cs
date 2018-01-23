namespace NuClear.ValidationRules.Replication.Dto
{
    public sealed class AdvertisementDto
    {
        public long Id { get; set; }
        public long FirmId { get; set; }
        public string Name { get; set ; }
        public int StateCode { get; set; }

        public long Offset { get; set; }
    }
}