using System;

namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class Bill
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long PayablePlan { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDate { get; set; }
        public int BillType { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
