using System;

namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class ReleaseInfo
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
        public bool IsBeta { get; set; }
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
    }
}