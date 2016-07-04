using System;

namespace NuClear.ValidationRules.Storage.Model.AccountRules.Facts
{
    public sealed class Lock
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
    }
}