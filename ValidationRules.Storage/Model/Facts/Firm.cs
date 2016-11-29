namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Firm
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
        public string Name { get; set; }

        // нужно уметь различать между собой все статусы
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsClosedForAscertainment { get; set; }
    }
}