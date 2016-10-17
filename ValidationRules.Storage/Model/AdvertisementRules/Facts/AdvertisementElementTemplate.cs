namespace NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts
{
    public sealed class AdvertisementElementTemplate
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public bool IsRequired { get; set; }
        public bool NeedsValidation { get; set; }
    }
}
