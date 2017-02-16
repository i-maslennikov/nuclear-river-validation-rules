namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class AdvertisementElementTemplate
    {
        public long Id { get; set; }

        public bool IsRequired { get; set; }
        public bool NeedsValidation { get; set; }
        public bool IsAdvertisementLink { get; set; }
    }
}
