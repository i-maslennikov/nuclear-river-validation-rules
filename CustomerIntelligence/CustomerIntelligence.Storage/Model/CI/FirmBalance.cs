namespace NuClear.CustomerIntelligence.Storage.Model.CI
{
    public sealed class FirmBalance
    {
        public long ProjectId { get; set; }

        public long FirmId { get; set; }

        public long AccountId { get; set; }

        public decimal Balance { get; set; }
    }
}