namespace NuClear.CustomerIntelligence.Storage.Model.Facts
{
    public sealed class CategoryOrganizationUnit
    {
        public long Id { get; set; }

        public long CategoryId { get; set; }

        public long CategoryGroupId { get; set; }

        public long OrganizationUnitId { get; set; }
    }
}