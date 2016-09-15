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
        public long DestOrganizationUnitId { get; set; }
    }

    // вспомогательный, служит для преобразования DestOrganizationUnitId->ProjectId
    public sealed class Project
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
    }

    public sealed class OrderPosition
    {
        public long Id { get; set; }
        public long OrderId { get; set; }

        public long PricePositionId { get; set; }
    }

    public sealed class OrderPositionAdvertisement
    {
        public long Id { get; set; }
        public long OrderPositionId { get; set; }

        public long PositionId { get; set; }
        public long? AdvertisementId { get; set; }
    }

    public sealed class PricePosition
    {
        public long Id { get; set; }

        public long PositionId { get; set; }
    }

    public sealed class Position
    {
        public long Id { get; set; }

        public long? AdvertisementTemplateId { get; set; }

        public string Name { get; set; }

        public bool IsCompositionOptional { get; set; }

        public long? ChildPositionId { get; set; }
    }

    public sealed class AdvertisementTemplate
    {
        public long Id { get; set; }
        public long DummyAdvertisementId { get; set; }
        public bool IsAdvertisementRequired { get; set; }
        public bool IsAllowedToWhiteList { get; set; }
    }

    public sealed class Advertisement
    {
        public long Id { get; set; }
        public long? FirmId { get; set; }
        public long AdvertisementTemplateId { get; set; }

        public string Name { get; set; }

        public bool IsSelectedToWhiteList { get; set; }
        public bool IsDeleted { get; set; }
    }

    public sealed class Firm
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public sealed class AdvertisementElement
    {
        public long Id { get; set; }
        public long AdvertisementId { get; set; }
        public long AdvertisementElementTemplateId { get; set; }

        public bool IsEmpty { get; set; }
        public int Status { get; set; }
    }

    public sealed class AdvertisementElementTemplate
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public bool IsRequired { get; set; }
        public bool NeedsValidation { get; set; }
    }
}
