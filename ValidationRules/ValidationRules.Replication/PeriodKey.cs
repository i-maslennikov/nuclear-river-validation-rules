using System;

namespace NuClear.ValidationRules.Replication
{
    public sealed class PeriodKey
    {
        public long ProjectId { get; set; }
        public DateTime Start { get; set; }
    }
}