using System;

namespace NuClear.ValidationRules.WebApp.Entity
{
    public class Order
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDateFact { get; set; }
        public long OwnerCode { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public int WorkflowStepId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
