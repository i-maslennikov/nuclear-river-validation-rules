using System;

namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class Ruleset
    {
        public long Id { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        public sealed class AssociatedRule
        {
            public long RulesetId { get; set; }
            public long PrincipalNomenclatureId { get; set; }
            public long AssociatedNomenclatureId { get; set; }
            public bool ConsideringBindingObject { get; set; }
        }

        public sealed class DeniedRule
        {
            public long RulesetId { get; set; }
            public long NomenclatureId { get; set; }
            public long DeniedNomenclatureId { get; set; }
            public int BindingObjectStrategy { get; set; }
        }

        public sealed class QuantitativeRule
        {
            public long RulesetId { get; set; }
            public long NomenclatureCategoryCode { get; set; }
            public int Min { get; set; }
            public int Max { get; set; }
        }

        public sealed class RulesetProject
        {
            public long RulesetId { get; set; }
            public long ProjectId { get; set; }
        }
    }
}
