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

            /// <summary>
            /// Нормализованный (округлённый) период размещения купона.
            /// Начало размещения округляется до следющего месяца, если начало менее, чем за пять дней до следующего месяца.
            /// Например, для начала: 2017-01-31 -> 2017-02-01
            /// </summary>
            public DateTime BeginMonth { get; set; }

            /// <summary>
            /// Нормализованный (округлённый) период размещения купона.
            /// Окончание размещения округляется до начала месяца, если окончание менее, чем через пять дней от начала месяца.
            /// Например, для начала: 2017-01-03 -> 2017-01-01
            /// </summary>
            public DateTime EndMonth { get; set; }
        }
    }
}
