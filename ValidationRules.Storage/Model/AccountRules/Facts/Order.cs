using System;

namespace NuClear.ValidationRules.Storage.Model.AccountRules.Facts
{
    public sealed class Order
    {
        public long Id { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public long? BranchOfficeOrganizationUnitId { get; set; }
        public long? LegalPersonId { get; set; }
        public bool IsFreeOfCharge { get; set; }
        public string Number { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDate { get; set; }
    }
}
