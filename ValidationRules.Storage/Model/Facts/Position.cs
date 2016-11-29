namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Position
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public long? AdvertisementTemplateId { get; set; }
        public long BindingObjectType { get; set; }
        public int SalesModel { get; set; }
        public int PositionsGroup { get; set; }


        public bool IsCompositionOptional { get; set; }
        public bool IsControlledByAmount { get; set; }
        public bool IsComposite { get; set; }

        public long CategoryCode { get; set; }
        public int Platform { get; set; }

        public bool IsDeleted { get; set; }
    }
}