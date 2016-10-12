using System;

namespace NuClear.ValidationRules.Storage.Model.ProjectRules.Facts
{
    public sealed class CostPerClickCategoryRestriction
    {
        public long ProjectId { get; set; }
        public long CategoryId { get; set; }
        public DateTime Begin { get; set; }
        public decimal MinCostPerClick { get; set; }
    }
}