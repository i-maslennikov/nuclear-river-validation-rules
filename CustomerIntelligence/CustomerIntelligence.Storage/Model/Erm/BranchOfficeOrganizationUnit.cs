namespace NuClear.CustomerIntelligence.Storage.Model.Erm
{
    public sealed class BranchOfficeOrganizationUnit
    {
        public BranchOfficeOrganizationUnit()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public long OrganizationUnitId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}