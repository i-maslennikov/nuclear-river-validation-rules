namespace NuClear.ValidationRules.Storage.Model.ProjectRules.Facts
{
    public sealed class OrderPositionAdvertisement
    {
        public long Id { get; set; }
        public long OrderPositionId { get; set; }
        public long PositionId { get; set; }
        public long? FirmAddressId { get; set; }
        public long? CategoryId { get; set; }
    }
}