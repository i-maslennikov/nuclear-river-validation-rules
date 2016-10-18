namespace NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts
{
    public sealed class OrderPositionAdvertisement
    {
        public long Id { get; set; }
        public long OrderPositionId { get; set; }

        public long PositionId { get; set; }
        public long? AdvertisementId { get; set; }
    }
}