using System;

namespace NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts
{
    public sealed class Bill
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public decimal PayablePlan { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
    }
}
