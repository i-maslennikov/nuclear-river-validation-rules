namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class Firm
    {
        public long Id { get; set; }
        public bool ClosedForAscertainment { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }
}