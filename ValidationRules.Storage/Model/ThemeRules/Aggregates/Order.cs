using System;

namespace NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates
{
    public sealed class Order
    {
        public long Id { get; set; }

        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDateFact { get; set; }

        public long ProjectId { get; set; }

        public bool IsSelfAds { get; set; }

        public sealed class OrderTheme
        {
            public long OrderId { get; set; }
            public long ThemeId { get; set; }
        }
    }
}