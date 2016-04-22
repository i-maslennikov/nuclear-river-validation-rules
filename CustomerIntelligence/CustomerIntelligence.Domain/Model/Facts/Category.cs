namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class Category
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public long? ParentId { get; set; }
    }
}