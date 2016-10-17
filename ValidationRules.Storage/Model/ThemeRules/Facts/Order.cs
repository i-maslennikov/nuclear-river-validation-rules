using System;

namespace NuClear.ValidationRules.Storage.Model.ThemeRules.Facts
{
    public sealed class Order
    {
        public long Id { get; set; }

        public string Number { get; set; }

        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDateFact { get; set; }
        public long DestOrganizationUnitId { get; set; }

        public bool IsSelfAds { get; set; }
    }
}