namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public class CategoryOrganizationUnit
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public long OrganizationUnitId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}