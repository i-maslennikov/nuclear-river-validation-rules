namespace NuClear.CustomerIntelligence.Querying.Tests.Model.CustomerIntelligence
{
    public sealed class FirmBalance
    {
        public long AccountId { get; set; }
        public long FirmId { get; set; }

        public long ProjectId { get; set; }
        public decimal Balance { get; set; }
    }
}