using System;

namespace NuClear.CustomerIntelligence.Storage.Model.Erm
{
    public sealed class Order
    {
        public Order()
        {
            IsActive = true;
        }

        public long Id { get; set; }

        public DateTimeOffset EndDistributionDateFact { get; set; }

        public int WorkflowStepId { get; set; }

        public long FirmId { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
    }
}