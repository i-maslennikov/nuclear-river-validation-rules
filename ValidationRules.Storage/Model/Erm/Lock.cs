using System;

namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class Lock
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long AccountId { get; set; }
        public decimal PlannedAmount { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class Limit
    {
        public long Id { get; set; }

        public long AccountId { get; set; }
        public DateTime StartPeriodDate { get; set; }
        public decimal Amount { get; set; }

        public int Status { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

    public sealed class ReleaseWithdrawal
    {
        public long Id { get; set; }

        public long OrderPositionId { get; set; }
        public DateTime ReleaseBeginDate { get; set; }
        public decimal AmountToWithdraw { get; set; }
    }
}
