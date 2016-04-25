using System;

namespace NuClear.ValidationRules.Domain.Model
{
    public sealed class PeriodKey
    {
        public long OrganizationUnitId { get; set; }
        public DateTime Start { get; set; }
    }
}