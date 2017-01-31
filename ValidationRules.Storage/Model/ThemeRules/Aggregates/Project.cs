using System;

namespace NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates
{
    public sealed class Project
    {
        public long Id { get; set; }
        public sealed class ProjectDefaultTheme
        {
            public long ProjectId { get; set; }
            public long ThemeId { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }
    }
}