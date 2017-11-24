using System;

namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class AccountDetail
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public long? OrderId { get; set; }
        public DateTime? PeriodStartDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}