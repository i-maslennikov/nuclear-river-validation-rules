namespace NuClear.CustomerIntelligence.Querying.Tests.Model
{
    public sealed class Category
    {
        public long ProjectId { get; set; }

        public long CategoryId { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public long? ParentId { get; set; }
    }
}