using System;

namespace NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates
{
    public sealed class Firm
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public sealed class WhiteListDistributionPeriod
        {
            public long FirmId { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public long? AdvertisementId { get; set; }
            public long? ProvidedByOrderId { get; set; }
        }
    }
}