namespace NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts
{
    public sealed class AdvertisementElement
    {
        public long Id { get; set; }
        public long AdvertisementId { get; set; }
        public long AdvertisementElementTemplateId { get; set; }

        public bool IsEmpty { get; set; }
        public int Status { get; set; }
    }
}