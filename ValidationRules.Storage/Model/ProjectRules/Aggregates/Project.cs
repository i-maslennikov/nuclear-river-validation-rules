using System;

namespace NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates
{
    public sealed class Project
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public class Category
        {
            public long ProjectId { get; set; }
            public long CategoryId { get; set; }
        }

        public sealed class CostPerClickRestriction
        {
            public long ProjectId { get; set; }
            public long CategoryId { get; set; }
            public decimal Minimum { get; set; }
            public DateTime Begin { get; set; }
            public DateTime End { get; set; }
        }

        public sealed class SalesModelRestriction
        {
            public long ProjectId { get; set; }
            public long CategoryId { get; set; }
            public int SalesModel { get; set; }
            public DateTime Begin { get; set; }
            public DateTime End { get; set; }
        }

        public sealed class NextRelease
        {
            public long ProjectId { get; set; }
            public DateTime Date { get; set; }
        }
    }
}
