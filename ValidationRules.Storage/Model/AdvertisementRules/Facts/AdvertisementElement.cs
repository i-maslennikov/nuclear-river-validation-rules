using System;

namespace NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts
{
    public sealed class AdvertisementElement
    {
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