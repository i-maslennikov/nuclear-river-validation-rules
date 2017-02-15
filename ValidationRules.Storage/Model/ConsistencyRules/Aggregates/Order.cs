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

    public enum InvalidFirmAddressState
    {
        NotSet = 0,
        Deleted,
        NotActive,
        ClosedForAscertainment,
        NotBelongToFirm
    }

    public enum InvalidCategoryFirmAddressState
    {
        NotSet = 0,
        CategoryNotBelongsToAddress
    }

    public enum InvalidCategoryState
    {
        NotSet = 0,
        Inactive,
        NotBelongToFirm
    }

    public enum DealState
    {
        NotSet = 0,
        Missing,
        Inactive
    }

    public sealed class Order
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public DateTime BeginDistribution { get; set; }
        public DateTime EndDistributionFact { get; set; }
        public DateTime EndDistributionPlan { get; set; }

        public class InactiveReference
        {
            public long OrderId { get; set; }
            public bool Deal { get; set; }
            public bool LegalPerson { get; set; }
            public bool LegalPersonProfile { get; set; }
            public bool BranchOfficeOrganizationUnit { get; set; }
            public bool BranchOffice { get; set; }
        }

        public class InvalidFirm
        {
            public long OrderId { get; set; }
            public long FirmId { get; set; }
            public InvalidFirmState State { get; set; }
        }

        public class InvalidFirmAddress
        {
            public long OrderId { get; set; }
            public long FirmAddressId { get; set; }
            public long OrderPositionId { get; set; }
            public long PositionId { get; set; }
            public InvalidFirmAddressState State { get; set; }
        }

        public class InvalidCategoryFirmAddress
        {
            public long OrderId { get; set; }
            public long FirmAddressId { get; set; }
            public long CategoryId { get; set; }
            public long OrderPositionId { get; set; }
            public long PositionId { get; set; }
            public InvalidCategoryFirmAddressState State { get; set; }
        }

        public class InvalidCategory
        {
            public long OrderId { get; set; }
            public long CategoryId { get; set; }
            public long OrderPositionId { get; set; }
            public long PositionId { get; set; }
            public InvalidCategoryState State { get; set; }
            public bool MayNotBelongToFirm { get; set; }
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
        }

        public class LegalPersonProfileWarrantyExpired
        {
            public long OrderId { get; set; }
            public long LegalPersonProfileId { get; set; }
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
            public bool ReleaseCountPlan { get; set; }
            public bool Deal { get; set; }
        }
    }
}
