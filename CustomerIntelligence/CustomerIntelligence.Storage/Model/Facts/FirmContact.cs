namespace NuClear.CustomerIntelligence.Storage.Model.Facts
{
    public sealed class FirmContact
    {
        public long Id { get; set; }

        public bool HasPhone { get; set; }

        public bool HasWebsite { get; set; }

        public long FirmAddressId { get; set; }
    }
}