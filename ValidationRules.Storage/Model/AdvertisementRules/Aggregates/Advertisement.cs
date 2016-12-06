using System;

namespace NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates
{
    public sealed class Advertisement
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long FirmId { get; set; }
        public bool IsSelectedToWhiteList { get; set; }

        public sealed class AdvertisementWebsite
        {
            public long AdvertisementId { get; set; }
            public string Website { get; set; }
        }

        public sealed class RequiredElementMissing
        {
            public long AdvertisementId { get; set; }

            public long AdvertisementElementId { get; set; }
            public long AdvertisementElementTemplateId { get; set; }
        }

        public enum ReviewStatus
        {
            NotSet = 0,
            Invalid,
            Draft,
        }

        public sealed class ElementNotPassedReview
        {
            public long AdvertisementId { get; set; }

            public long AdvertisementElementId { get; set; }
            public long AdvertisementElementTemplateId { get; set; }

            public ReviewStatus Status { get; set; }
        }

        public sealed class Coupon
        {
            public long AdvertisementId { get; set; }

            public long AdvertisementElementId { get; set; }

            public int DaysTotal { get; set; }
            public int DaysFromMonthBeginToCouponEnd { get; set; }
            public int DaysFromCouponBeginToMonthEnd { get; set; }

            public DateTime BeginMonth { get; set; }
            public DateTime EndMonth { get; set; }
        }
    }
}
