using System;

namespace NuClear.ValidationRules.Storage.Model.Erm
{
    public class CostPerClickCategoryRestriction
    {
        public long ProjectId { get; set; }
        public long CategoryId { get; set; }
        public DateTime BeginningDate { get; set; }
        public decimal MinCostPerClick { get; set; }
    }
}