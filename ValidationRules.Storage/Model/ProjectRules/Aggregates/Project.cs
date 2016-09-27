using System;

namespace NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates
{
    public sealed class Project
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public sealed class MinimumCostPerClick
        {
            public long ProjectId { get; set; }
            public long CategoryId { get; set; }
            public decimal Restriction { get; set; }
        }

        public sealed class NextRelease
        {
            public long ProjectId { get; set; }
            public DateTime Date { get; set; }
        }

        public class Category
        {
            public long ProjectId { get; set; }
            public long CategoryId { get; set; }
        }
    }
}