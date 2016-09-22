using System;

namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class Bargain
    {
        public long Id { get; set; }
        public DateTime SignedOn { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
