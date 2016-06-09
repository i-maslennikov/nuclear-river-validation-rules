namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class PricePositionNotActive
    {
        public long Id { get; set; }
        public long PriceId { get; set; }
        public long PositionId { get; set; }
    }
}