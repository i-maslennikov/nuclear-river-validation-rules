using System;

namespace NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates
{
    public sealed class Order
    {
        public long Id { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDatePlan { get; set; }

        public sealed class MissingOrderPositionAdvertisement
        {
            public long OrderId { get; set; }

            public long OrderPositionId { get; set; }
            public long CompositePositionId { get; set; }

            public long PositionId { get; set; }
        }
    }
}
