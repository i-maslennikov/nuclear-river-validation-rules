namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class FirmAddress
    {
        public long Id { get; set; }
        public long FirmId { get; set; }
        public string Name { get; set; }

        public bool IsLocatedOnTheMap { get; set; }

        // нужно уметь различать между собой все статусы
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsClosedForAscertainment { get; set; }
    }
}