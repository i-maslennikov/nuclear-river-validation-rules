using System;

namespace NuClear.ValidationRules.WebApp.Entity
{
    public class ReleaseInfo
    {
        public DateTime PeriodEndDate { get; set; }
        public long OrganizationUnitId { get; set; }
        public bool IsBeta { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int Status { get; set; }
    }
}