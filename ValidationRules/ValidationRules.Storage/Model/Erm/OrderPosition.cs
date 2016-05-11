namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class OrderPosition
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long PricePositionId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
