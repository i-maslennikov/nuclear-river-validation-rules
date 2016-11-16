using System;

namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class UnlimitedOrder
    {
        public long OrderId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public bool IsActive { get; set; }
    }
}
