using System;

namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class Lock
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
