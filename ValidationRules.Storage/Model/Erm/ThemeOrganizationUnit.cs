namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class ThemeOrganizationUnit
    {
        public long Id { get; set; }
        public long ThemeId { get; set; }
        public long OrganizationUnitId { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}