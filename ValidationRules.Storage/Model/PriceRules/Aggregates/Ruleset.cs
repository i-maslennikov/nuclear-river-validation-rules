using System;

namespace NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates
{
    public sealed class Ruleset
    {
        public long Id { get; set; }

        public sealed class AdvertisementAmountRestriction
        {
            public long RulesetId { get; set; }
            public long ProjectId { get; set; }

            public DateTime Begin { get; set; }
            public DateTime End { get; set; }

            public long CategoryCode { get; set; }
            public string CategoryName { get; set; }
            public int Min { get; set; }
            public int Max { get; set; }
        }

    }
}