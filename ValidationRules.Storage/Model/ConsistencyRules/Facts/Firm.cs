namespace NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts
{
    public sealed class Firm
    {
        public long Id { get; set; }
        public bool ClosedForAscertainment { get; set; }
        public bool IsHidden { get; set; }
        public bool IsDeleted { get; set; }
    }
}