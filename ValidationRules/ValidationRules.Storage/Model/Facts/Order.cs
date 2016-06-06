using System;

namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Order
    {
        public long Id { get; set; }
        public long FirmId { get; set; }
        public long DestProjectId { get; set; }
        public long SourceProjectId { get; set; }
        public long OwnerId { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDateFact { get; set; }
        public int BeginReleaseNumber { get; set; }
        public int EndReleaseNumberFact { get; set; }
        public int EndReleaseNumberPlan { get; set; }
        public int WorkflowStepId { get; set; }
        public string Number { get; set; }
    }
}
