using System;

namespace NuClear.ValidationRules.Storage.Model.Erm
{
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
}