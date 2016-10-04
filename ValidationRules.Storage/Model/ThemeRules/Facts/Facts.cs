using System;

namespace NuClear.ValidationRules.Storage.Model.ThemeRules.Facts
{
    public sealed class Theme
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public DateTime BeginDistribution { get; set; }
        public DateTime EndDistribution { get; set; }

        public bool IsDefault { get; set; }
    }

    public sealed class ThemeCategory
    {
        public long Id { get; set; }
        public long ThemeId { get; set; }
        public long CategoryId { get; set; }
    }

    public sealed class ThemeOrganizationUnit
    {
        public long Id { get; set; }
        public long ThemeId { get; set; }
        public long OrganizationUnitId { get; set; }
    }

    public sealed class Category
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public bool IsInvalid { get; set; }
    }

    public sealed class Order
    {
        public long Id { get; set; }

        public string Number { get; set; }

        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDateFact { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public long DestOrganizationUnitId { get; set; }

        public bool IsSelfAds { get; set; }
    }

    // вспомогательный, служит для преобразования OrganizationUnitId->ProjectId
    public sealed class Project
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }

        public string Name { get; set; }
    }

    public sealed class OrderPosition
    {
        public long Id { get; set; }

        public long OrderId { get; set; }
    }

    public sealed class OrderPositionAdvertisement
    {
        public long Id { get; set; }
        public long OrderPositionId { get; set; }
        public long ThemeId { get; set; }
    }
}
