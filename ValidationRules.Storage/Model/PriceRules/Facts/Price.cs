using System;

namespace NuClear.ValidationRules.Storage.Model.PriceRules.Facts
{
    public sealed class Price
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
        public DateTime BeginDate { get; set; }
    }
}
