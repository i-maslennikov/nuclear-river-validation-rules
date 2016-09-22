using System;

namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class Price
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
        public DateTime BeginDate { get; set; }
        public bool IsPublished { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
