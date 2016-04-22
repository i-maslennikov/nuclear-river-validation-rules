namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class Territory
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long OrganizationUnitId { get; set; }
    }
}