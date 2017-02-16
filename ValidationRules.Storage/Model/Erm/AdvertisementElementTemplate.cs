namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class AdvertisementElementTemplate
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public bool IsRequired { get; set; }
        public bool NeedsValidation  { get; set; }
        public bool IsAdvertisementLink { get; set; }

        public bool IsDeleted { get; set; }
    }
}
