using System;

namespace NuClear.ValidationRules.Domain.Model.Facts
{
    public sealed class Price : IErmFactObject
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
        public DateTime BeginDate { get; set; }
    }
}
