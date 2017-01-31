using System;

namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Lock
    {
        public long Id { get; set; }

        public long OrderId { get; set; }
        public bool IsOrderFreeOfCharge { get; set; }
        public long AccountId { get; set; }
        public DateTime Start { get; set; }
        public decimal Amount { get; set; }
    }
}