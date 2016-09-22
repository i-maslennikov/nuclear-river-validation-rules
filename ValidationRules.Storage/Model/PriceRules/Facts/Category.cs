namespace NuClear.ValidationRules.Storage.Model.PriceRules.Facts
{
    public sealed class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public long? L1Id { get; set; }
        public long? L2Id { get; set; }
        public long? L3Id { get; set; }
    }
}
