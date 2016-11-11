namespace NuClear.ValidationRules.WebApp.DataAccess.Entity
{
    public class Project
    {
        public long Id { get; set; }
        public string DisplayName { get; set; }

        public long? OrganizationUnitId { get; set; }
        public bool IsActive { get; set; }
    }
}