using System;

namespace NuClear.ValidationRules.Replication
{
    public sealed class PeriodKey
    {
        public long OrganizationUnitId { get; set; }
        public DateTime Start { get; set; }
    }
}