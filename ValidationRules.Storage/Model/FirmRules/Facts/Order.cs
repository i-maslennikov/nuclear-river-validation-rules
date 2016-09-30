using System;

namespace NuClear.ValidationRules.Storage.Model.FirmRules.Facts
{
    public sealed class Order
    {
        public long Id { get; set; }
        public long FirmId { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public string Number { get; set; }
        public DateTime BeginDistribution { get; set; }
        public DateTime EndDistributionFact { get; set; }
        public int WorkflowStep { get; set; }
    }
}
