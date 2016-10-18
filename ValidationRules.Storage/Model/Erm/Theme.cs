using System;

namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public sealed class Theme
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public DateTime BeginDistribution { get; set; }
        public DateTime EndDistribution { get; set; }

        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}