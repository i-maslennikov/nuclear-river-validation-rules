using System;

namespace NuClear.ValidationRules.Storage.Model.AccountRules.Facts
{
    public sealed class Order
    {
        public long Id { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public long? AccountId { get; set; }
        public string Number { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDate { get; set; }
    }
}
