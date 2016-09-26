using System;

namespace NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates
{
    public sealed class Order
    {
        public long Id { get; set; }
        public string Number { get; set; }

        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDatePlan { get; set; }
        public long ProjectId { get; set; }

        public sealed class RequiredAdvertisementMissing
        {
            public long OrderId { get; set; }

            public long OrderPositionId { get; set; }
            public long CompositePositionId { get; set; }

            public long PositionId { get; set; }
        }

        public sealed class RequiredLinkedObjectCompositeMissing
        {
            public long OrderId { get; set; }

            public long OrderPositionId { get; set; }
            public long CompositePositionId { get; set; }

            public long PositionId { get; set; }
        }

        public sealed class AdvertisementDeleted
        {
            public long OrderId { get; set; }

            public long OrderPositionId { get; set; }
            public long PositionId { get; set; }

            public long AdvertisementId { get; set; }
        }

        public sealed class AdvertisementMustBelongToFirm
        {
            public long OrderId { get; set; }

            public long OrderPositionId { get; set; }
            public long PositionId { get; set; }

            public long AdvertisementId { get; set; }

            public long FirmId { get; set; }
        }

        public sealed class AdvertisementIsDummy
        {
            public long OrderId { get; set; }

            public long PositionId { get; set; }
        }

        public sealed class OrderAdvertisement
        {
            public long OrderId { get; set; }
            public long AdvertisementId { get; set; }
        }

        public sealed class WhiteListAdvertisement
        {
            public long OrderId { get; set; }

            public DateTime PeriodStart { get; set; }
            public DateTime PeriodEnd { get; set; }

            public long FirmId { get; set; }

            public long? AdvertisementId { get; set; }
        }
    }
}
