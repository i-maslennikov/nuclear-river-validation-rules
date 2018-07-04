using System;
using System.Collections.Generic;

namespace NuClear.ValidationRules.Replication.Dto
{
    public sealed class RulesetDto
    {
        public long Id { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<AssociatedRule> AssociatedRules { get; set; }
        public IEnumerable<DeniedRule> DeniedRules { get; set; }
        public IEnumerable<QuantitativeRule> QuantitativeRules { get; set; }
        public IEnumerable<long> Projects { get; set; }

        public sealed class AssociatedRule
        {
            public long NomeclatureId { get; set; }
            public long AssociatedNomenclatureId { get; set; }
            public bool ConsideringBindingObject { get; set; }
        }

        public sealed class DeniedRule
        {
            public long NomeclatureId { get; set; }
            public long DeniedNomenclatureId { get; set; }
            public int BindingObjectStrategy { get; set; }
        }

        public sealed class QuantitativeRule
        {
            public long NomenclatureCategoryCode { get; set; }
            public int Min { get; set; }
            public int Max { get; set; }
        }
    }
}