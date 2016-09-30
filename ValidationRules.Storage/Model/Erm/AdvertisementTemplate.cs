namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class AdvertisementTemplate
    {
        public long Id { get; set; }
        public long? DummyAdvertisementId { get; set; }

        public bool IsAdvertisementRequired { get; set; }
        public bool IsAllowedToWhiteList { get; set; }

        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
    }
}
