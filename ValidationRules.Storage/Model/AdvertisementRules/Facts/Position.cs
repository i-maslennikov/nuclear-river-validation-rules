namespace NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts
{
    public sealed class Position
    {
        public long Id { get; set; }

        public long? AdvertisementTemplateId { get; set; }

        public string Name { get; set; }

        public bool IsCompositionOptional { get; set; }
        public long CategoryCode { get; set; }

        public long? ChildPositionId { get; set; }
    }
}