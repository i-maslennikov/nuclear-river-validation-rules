using System;

namespace NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates
{
    public sealed class Order
    {
        public long Id { get; set; }
        public long DestProjectId { get; set; }
        public long SourceProjectId { get; set; }
        public long? AccountId { get; set; }
        public string Number { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDate { get; set; }
    }
}