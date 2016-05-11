namespace NuClear.CustomerIntelligence.Storage.Model.Erm
{
    public sealed class Project
    {
        public Project()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public long? OrganizationUnitId { get; set; }

        public bool IsActive { get; set; }
    }
}