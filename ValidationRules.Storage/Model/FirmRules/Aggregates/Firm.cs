using System;

namespace NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates
{
    public sealed class Firm
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }

        public sealed class AdvantageousPurchasePositionDistributionPeriod
        {
            public long FirmId { get; set; }
            public long Scope { get; set; }
            public bool HasPosition { get; set; }
            public DateTime Begin { get; set; }
            public DateTime End { get; set; }
        }
    }
}