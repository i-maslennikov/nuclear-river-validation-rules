using System;

namespace NuClear.ValidationRules.Storage.Model.WebApp
{
    public sealed class Lock
    {
        public long Id { get; set; }
        public DateTime Expires { get; set; }
        public bool InUse { get; set; }
        public bool IsNew { get; set; }
    }
}
