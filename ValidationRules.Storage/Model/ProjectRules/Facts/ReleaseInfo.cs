using System;

namespace NuClear.ValidationRules.Storage.Model.ProjectRules.Facts
{
    public sealed class ReleaseInfo
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
        public DateTime PeriodEndDate { get; set; }
    }
}