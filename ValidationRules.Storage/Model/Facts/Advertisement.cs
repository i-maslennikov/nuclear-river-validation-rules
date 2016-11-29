namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Advertisement
    {
        public long Id { get; set; }
        public long? FirmId { get; set; }
        public long AdvertisementTemplateId { get; set; }

        public string Name { get; set; }

        public bool IsSelectedToWhiteList { get; set; }
        public bool IsDeleted { get; set; }
    }
}