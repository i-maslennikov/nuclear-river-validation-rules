namespace NuClear.ValidationRules.Storage.Model.ProjectRules.Facts
{
    public sealed class OrderPosition
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long PricePositionId { get; set; }
    }
}