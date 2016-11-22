using System;

namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Theme
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public DateTime BeginDistribution { get; set; }
        public DateTime EndDistribution { get; set; }

        public bool IsDefault { get; set; }
    }
}