using System;

namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class AdvertisementElement
    {
        public long Id { get; set; }
        public long AdvertisementId { get; set; }
        public long AdvertisementElementTemplateId { get; set; }

        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? FileId { get; set; }
        public string Text { get; set; }

        public bool IsDeleted { get; set; }
    }
}
