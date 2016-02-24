using System;

namespace NuClear.ValidationRules.Domain.Model
{
    public sealed class PeriodId
    {
        public long OrganizationUnitId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}