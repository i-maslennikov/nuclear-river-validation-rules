namespace NuClear.ValidationRules.Storage.Model.PriceRules.Facts
{
    public sealed class PricePositionNotActive
    {
        public long Id { get; set; }
        public long PriceId { get; set; }
        public long PositionId { get; set; }
    }
}