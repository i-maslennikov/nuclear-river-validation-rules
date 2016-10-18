using System;

namespace NuClear.ValidationRules.Storage.Model.ProjectRules.Facts
{
    public class SalesModelCategoryRestriction
    {
        public long ProjectId { get; set; }
        public long CategoryId { get; set; }
        public DateTime Begin { get; set; }
        public int SalesModel { get; set; }
    }
}