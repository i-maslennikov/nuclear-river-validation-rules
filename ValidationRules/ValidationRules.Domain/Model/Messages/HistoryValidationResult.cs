using System;

namespace NuClear.ValidationRules.Domain.Model.Messages
{
    public sealed class HistoryValidationResult
    {
        public long OrderId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int MessageType { get; set; }

        public string OrderVersion { get; set; }
        public bool IsFailed { get; set; }
        public string MessageParams { get; set; }
    }
}
