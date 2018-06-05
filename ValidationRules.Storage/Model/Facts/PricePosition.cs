namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class PricePosition
    {
        public long Id { get; set; }
        public long PriceId { get; set; }
        public long PositionId { get; set; }
        public bool IsActiveNotDeleted { get; set; }
    }
}