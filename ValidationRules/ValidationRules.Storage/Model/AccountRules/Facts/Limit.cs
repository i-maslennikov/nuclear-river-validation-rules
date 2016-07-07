using System;

namespace NuClear.ValidationRules.Storage.Model.AccountRules.Facts
{
    public sealed class Limit
    {
        public long Id { get; set; }

        public long AccountId { get; set; }
        public DateTime Start { get; set; }
        public decimal Amount { get; set; }
    }
}