using System;

namespace NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates
{
    public sealed class Theme
    {
        public long Id { get; set; }

        public DateTime BeginDistribution { get; set; }
        public DateTime EndDistribution { get; set; }

        public bool IsDefault { get; set; }

        public sealed class InvalidCategory
        {
            public long ThemeId { get; set; }
            public long CategoryId { get; set; }
        }
    }
}