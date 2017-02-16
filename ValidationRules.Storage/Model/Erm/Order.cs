using System;

namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class Order
    {
        public const int OrderTypeSelfAds = 2;
        public const int OrderTypeSocialAds = 7;
        public const int OrderTypeCompensation = 9;

        public static readonly int[] FreeOfChargeTypes = { OrderTypeSelfAds, OrderTypeSocialAds, OrderTypeCompensation };

        private const int OrderStateArchive = 6;
        private const int OrderStateRejected = 3;

        public static readonly int[] FilteredStates = { OrderStateArchive, OrderStateRejected };

        public long Id { get; set; }
        public long FirmId { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public long? LegalPersonId { get; set; }
        public long? LegalPersonProfileId { get; set; }
        public long? BranchOfficeOrganizationUnitId { get; set; }
        public long? InspectorCode { get; set; }
        public long? CurrencyId { get; set; }
        public long? BargainId { get; set; }
        public long? DealId { get; set; }
        public long OwnerCode { get; set; }
        public DateTime SignupDate { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDateFact { get; set; }
        public DateTime EndDistributionDatePlan { get; set; }
        public int ReleaseCountPlan { get; set; }
        public int WorkflowStepId { get; set; }
        public int OrderType { get; set; }
        public string Number { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
