namespace NuClear.CustomerIntelligence.Storage.Model.Facts
{
    public sealed class FirmAddress
    {
        public long Id { get; set; }

        public long FirmId { get; set; }

        public long? TerritoryId { get; set; }
    }
}