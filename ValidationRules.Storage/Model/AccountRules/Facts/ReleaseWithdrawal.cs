using System;

namespace NuClear.ValidationRules.Storage.Model.AccountRules.Facts
{
    public sealed class ReleaseWithdrawal
    {
        public long Id { get; set; }

        public long OrderPositionId { get; set; }
        public DateTime Start { get; set; }
        public decimal Amount { get; set; }
    }
}