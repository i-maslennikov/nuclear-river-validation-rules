namespace NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates
{
    public sealed class Advertisement
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public sealed class RequiredElementMissing
        {
            public long AdvertisementId { get; set; }

            public long AdvertisementElementId { get; set; }
            public long AdvertisementElementTemplateId { get; set; }
        }

        public sealed class ElementInvalid
        {
            public long AdvertisementId { get; set; }

            public long AdvertisementElementId { get; set; }
            public long AdvertisementElementTemplateId { get; set; }
        }

        public sealed class ElementDraft
        {
            public long AdvertisementId { get; set; }

            public long AdvertisementElementId { get; set; }
            public long AdvertisementElementTemplateId { get; set; }
        }
    }
}
