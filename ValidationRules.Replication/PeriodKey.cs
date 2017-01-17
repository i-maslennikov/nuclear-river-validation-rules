using System;

namespace NuClear.ValidationRules.Replication
{
    public struct PeriodKey
    {
        public long OrganizationUnitId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}