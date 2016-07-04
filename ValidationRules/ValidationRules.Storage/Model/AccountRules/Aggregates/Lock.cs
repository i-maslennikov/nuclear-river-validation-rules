using System;

namespace NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates
{
    public sealed class Lock
    {
        public long OrderId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}