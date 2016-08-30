namespace NuClear.ValidationRules.Storage.Model.PriceRules.Facts
{
    public sealed class Position
    {
        public long Id { get; set; }
        public long CategoryCode { get; set; }
        public bool IsControlledByAmount { get; set; }
        public bool IsComposite { get; set; }
        public string Name { get; set; }
    }
}
