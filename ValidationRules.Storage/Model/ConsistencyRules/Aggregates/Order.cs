using System;

namespace NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates
{
    public enum InvalidFirmState
    {
        NotSet = 0,
        Deleted,
        ClosedForever,
        ClosedForAscertainment
    }

    public sealed class Order
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public string Number { get; set; }
        public DateTime BeginDistribution { get; set; }
        public DateTime EndDistributionFact { get; set; }
        public DateTime EndDistributionPlan { get; set; }

        public class InvalidFirm
        {
            public long OrderId { get; set; }
            public long FirmId { get; set; }
            public InvalidFirmState State { get; set; }
            public string Name { get; set; }
        }

        public class InvalidBeginDistributionDate
        {
            public long OrderId { get; set; }
        }

        public class InvalidEndDistributionDate
        {
            public long OrderId { get; set; }
        }

        public class LegalPersonProfileBargainExpired
        {
            public long OrderId { get; set; }
            public long LegalPersonProfileId { get; set; }
            public string LegalPersonProfileName { get; set; }
        }

        public class LegalPersonProfileWarrantyExpired
        {
            public long OrderId { get; set; }
            public long LegalPersonProfileId { get; set; }
            public string LegalPersonProfileName { get; set; }
        }

        public class BargainSignedLaterThanOrder
        {
            public long OrderId { get; set; }
            public long BargainId { get; set; }
        }

        public class MissingBargainScan
        {
            public long OrderId { get; set; }
        }

        public class MissingOrderScan
        {
            public long OrderId { get; set; }
        }

        public class HasNoAnyLegalPersonProfile
        {
            public long OrderId { get; set; }
        }

        public class HasNoAnyPosition
        {
            public long OrderId { get; set; }
        }

        public class NoReleasesSheduled
        {
            public long OrderId { get; set; }
        }

        public class MissingBills
        {
            public long OrderId { get; set; }
        }

        public class InvalidBillsTotal
        {
            public long OrderId { get; set; }
        }

        public class InvalidBillsPeriod
        {
            public long OrderId { get; set; }
        }

        public class MissingRequiredField
        {
            public long OrderId { get; set; }
            public bool LegalPerson { get; set; }
            public bool LegalPersonProfile { get; set; }
            public bool BranchOfficeOrganizationUnit { get; set; }
            public bool Inspector { get; set; }
            public bool Currency { get; set; }
        }
    }
}
