using System;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public sealed class ValidationMessageRequest
    {
        public string UserAccount { get; set; }
        public long OrderId { get; set; }
        public DateTime PeriodStart { get; set; }
    }
}