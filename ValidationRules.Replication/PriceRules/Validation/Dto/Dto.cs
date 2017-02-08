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

    public sealed class Dto<TPosition1, TPosition2>
    {
        public long FirmId { get; set; }
        public DateTime Start { get; set; }
        public long OrganizationUnitId { get; set; }
        public Match Match { get; set; }
        public TPosition1 CausePosition { get; set; }
        public TPosition2 RelatedPosition { get; set; }
    }

    public enum Match
    {
        NoPosition = 0,
        DifferentBindingObject = 1,
        MatchedBindingObject = 2
    }
}