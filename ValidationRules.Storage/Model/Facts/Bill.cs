namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Bill
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public decimal PayablePlan { get; set; }
    }
}
