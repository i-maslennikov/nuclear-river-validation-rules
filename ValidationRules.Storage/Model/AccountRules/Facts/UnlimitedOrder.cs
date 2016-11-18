using System;

namespace NuClear.ValidationRules.Storage.Model.AccountRules.Facts
{
    public sealed class UnlimitedOrder
    {
        public long OrderId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}