using System;

namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class ReleaseInfo
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
        public DateTime PeriodEndDate { get; set; }
    }
}