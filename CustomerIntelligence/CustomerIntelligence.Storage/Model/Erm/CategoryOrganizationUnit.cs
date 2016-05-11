namespace NuClear.CustomerIntelligence.Storage.Model.Erm
{
    public sealed class CategoryOrganizationUnit
    {
        public CategoryOrganizationUnit()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public long CategoryId { get; set; }

        public long CategoryGroupId { get; set; }

        public long OrganizationUnitId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}