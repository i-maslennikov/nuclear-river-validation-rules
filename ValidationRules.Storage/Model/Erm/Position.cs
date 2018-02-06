namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class Position
    {
        public long Id { get; set; }
        public long CategoryCode { get; set; }
        public int SalesModel { get; set; }
        public bool IsControlledByAmount { get; set; }
        public bool IsCompositionOptional { get; set; }
        public int ContentSales { get; set; }
        public int BindingObjectTypeEnum { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public int PositionsGroup { get; set; }
    }
}
