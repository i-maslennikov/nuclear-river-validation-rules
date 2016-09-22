namespace NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts
{
    public sealed class ReleaseWithdrawal
    {
        public long Id { get; set; }
        public long OrderPositionId { get; set; }
        public decimal Amount { get; set; }
    }
}