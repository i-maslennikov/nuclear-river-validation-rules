using System;

namespace NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates
{
    public sealed class Order
    {
        public long Id { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDatePlan { get; set; }

        public sealed class MissingAdvertisementReference
        {
            public long OrderId { get; set; }

            public long OrderPositionId { get; set; }
            public long CompositePositionId { get; set; }

            public long PositionId { get; set; }
        }

        public sealed class MissingOrderPositionAdvertisement
        {
            public long OrderId { get; set; }

            public long OrderPositionId { get; set; }
            public long CompositePositionId { get; set; }

            public long PositionId { get; set; }
        }

        public sealed class AdvertisementNotBelongToFirm
        {
            public long OrderId { get; set; }
            public long AdvertisementId { get; set; }
            public long OrderPositionId { get; set; }
            public long PositionId { get; set; }

            public long ExpectedFirmId { get; set; }
            public long ActualFirmId { get; set; }
        }

        public sealed class AdvertisementFailedReview
        {
            public long OrderId { get; set; }
            public long AdvertisementId { get; set; }

            public int ReviewState { get; set; }
        }

        public enum AdvertisementReviewState
        {
            Draft = 1,
            Invalid = 2,
        }
    }
}
