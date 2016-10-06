namespace NuClear.ValidationRules.Storage.Model.ProjectRules.Facts
{
    public sealed class OrderPositionCostPerClick
    {
        public long OrderPositionId { get; set; }
        public long CategoryId { get; set; }
        public decimal Amount { get; set; }
    }
}