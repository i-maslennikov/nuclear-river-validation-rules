namespace NuClear.CustomerIntelligence.Domain.Model.Erm
{
    public sealed class CategoryGroup
    {
        public CategoryGroup()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public decimal Rate { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}