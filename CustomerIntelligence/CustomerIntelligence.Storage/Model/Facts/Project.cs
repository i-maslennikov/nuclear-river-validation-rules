namespace NuClear.CustomerIntelligence.Storage.Model.Facts
{
    public sealed class Project
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long OrganizationUnitId { get; set; }
    }
}