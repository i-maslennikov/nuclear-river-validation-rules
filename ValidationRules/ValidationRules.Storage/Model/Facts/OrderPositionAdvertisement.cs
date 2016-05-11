namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class OrderPositionAdvertisement
    {
        public long Id { get; set; }
        public long OrderPositionId { get; set; }
        public long PositionId { get; set; }
        public long? CategoryId { get; set; }
        public long? FirmAddressId { get; set; }
    }
}
