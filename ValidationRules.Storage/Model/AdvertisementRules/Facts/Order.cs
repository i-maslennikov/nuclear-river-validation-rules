using System;

namespace NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts
{
    public sealed class Order
    {
        public long Id { get; set; }
        public long FirmId { get; set; }
        public string Number { get; set; }

        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDatePlan { get; set; }
        public DateTime EndDistributionDateFact { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public int WorkflowStepId { get; set; }
    }
}