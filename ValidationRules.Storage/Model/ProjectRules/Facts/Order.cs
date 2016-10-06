using System;

namespace NuClear.ValidationRules.Storage.Model.ProjectRules.Facts
{
    public sealed class Order
    {
        public long Id { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public string Number { get; set; }
        public DateTime BeginDistribution { get; set; }
        public DateTime EndDistributionPlan { get; set; }
        public int WorkflowStep { get; set; }
    }
}
