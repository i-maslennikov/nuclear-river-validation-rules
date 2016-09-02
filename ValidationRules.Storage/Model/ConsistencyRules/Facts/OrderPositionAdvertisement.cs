namespace NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts
{
    public sealed class OrderPositionAdvertisement
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long? FirmAddressId { get; set; }
    }
}