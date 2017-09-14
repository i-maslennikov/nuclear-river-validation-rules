using System;

namespace NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates
{
    public sealed class Firm
    {
        public long Id { get; set; }

        public class CategoryPurchase
        {
            public long FirmId { get; set; }
            public long CategoryId { get; set; }

            public DateTime Begin { get; set; }
            public DateTime End { get; set; }
            public long Scope { get; set; }
        }
    }
}