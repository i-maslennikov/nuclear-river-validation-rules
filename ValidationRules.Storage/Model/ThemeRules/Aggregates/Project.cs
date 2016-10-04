namespace NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates
{
    public sealed class Project
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public sealed class ProjectTheme
        {
            public long ProjectId { get; set; }
            public long ThemeId { get; set; }
        }
    }
}