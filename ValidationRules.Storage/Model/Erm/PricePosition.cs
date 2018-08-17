namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class PricePosition
    {
        public long Id { get; set; }
        public long PriceId { get; set; }
        public long PositionId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
