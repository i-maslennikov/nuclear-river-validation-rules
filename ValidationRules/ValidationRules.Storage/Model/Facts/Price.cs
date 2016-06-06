using System;

namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Price
    {
        public long Id { get; set; }
        public long ProjectId { get; set; }
        public DateTime BeginDate { get; set; }
    }
}
