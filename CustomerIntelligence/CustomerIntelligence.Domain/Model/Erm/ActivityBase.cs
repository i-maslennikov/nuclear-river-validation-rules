using System;

namespace NuClear.CustomerIntelligence.Domain.Model.Erm
{
    public abstract class ActivityBase
    {
        public long Id { get; set; }
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }
    }
}
