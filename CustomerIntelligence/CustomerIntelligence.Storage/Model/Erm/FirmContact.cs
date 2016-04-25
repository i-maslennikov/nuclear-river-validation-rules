namespace NuClear.CustomerIntelligence.Storage.Model.Erm
{
    public sealed class FirmContact
    {
        public long Id { get; set; }

        public int ContactType { get; set; }

        public long? FirmAddressId { get; set; }
    }
}