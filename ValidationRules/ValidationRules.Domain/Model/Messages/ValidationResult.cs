using System;

namespace NuClear.ValidationRules.Domain.Model.Messages
{
    public sealed class ValidationResult
    {
        public long OrderId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int MessageType { get; set; }

        public long ProjectId { get; set; }
        public bool IsFailed { get; set; }
        public string MessageParams { get; set; }
    }
}
