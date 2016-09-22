namespace NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts
{
    public sealed class Firm
    {
        public long Id { get; set; }
        public bool IsClosedForAscertainment { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string Name { get; set; }
    }
}