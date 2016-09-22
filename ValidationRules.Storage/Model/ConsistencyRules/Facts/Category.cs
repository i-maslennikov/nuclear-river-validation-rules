namespace NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts
{
    public sealed class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsActiveNotDeleted { get; set; }
    }
}