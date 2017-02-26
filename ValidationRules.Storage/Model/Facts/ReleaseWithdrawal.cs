using System;

namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class ReleaseWithdrawal
    {
        public long OrderPositionId { get; set; }
        public DateTime Start { get; set; }
        public decimal Amount { get; set; }
    }
}