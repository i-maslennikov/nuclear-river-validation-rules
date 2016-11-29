namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class AdvertisementTemplate
    {
        public long Id { get; set; }
        public long DummyAdvertisementId { get; set; }
        public bool IsAdvertisementRequired { get; set; }
        public bool IsAllowedToWhiteList { get; set; }
    }
}