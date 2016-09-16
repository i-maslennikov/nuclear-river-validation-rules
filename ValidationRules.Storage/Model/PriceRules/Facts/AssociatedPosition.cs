namespace NuClear.ValidationRules.Storage.Model.PriceRules.Facts
{
    public sealed class AssociatedPosition
    {
        public long Id { get; set; }
        public long AssociatedPositionsGroupId { get; set; }
        public long PositionId { get; set; }
        public int ObjectBindingType { get; set; }
    }
}
