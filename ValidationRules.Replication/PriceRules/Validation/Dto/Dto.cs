using System;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation.Dto
{
    public sealed class Dto<TPosition>
    {
        public long FirmId { get; set; }
        public DateTime Start { get; set; }
        public long OrganizationUnitId { get; set; }
        public long Scope { get; set; }
        public TPosition Position { get; set; }
    }
}