namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class BranchOfficeOrganizationUnit
    {
        public long Id { get; set; }
        public long BranchOfficeId { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}