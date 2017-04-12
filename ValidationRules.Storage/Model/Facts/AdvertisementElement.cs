using System;

namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class AdvertisementElement
    {
        public const int Invalid = 2;
        public const int Draft = 3;

        public long Id { get; set; }
        public long AdvertisementId { get; set; }
        public long AdvertisementElementTemplateId { get; set; }

        public bool IsEmpty { get; set; }

        public string Text { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int Status { get; set; }
    }
}