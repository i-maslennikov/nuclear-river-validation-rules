namespace NuClear.ValidationRules.Storage.Model.Aggregates
{
    public sealed class RulesetRule
    {
        public long RulesetId { get; set; }

        public int RuleType { get; set; }
        public long DependentPositionId { get; set; }
        public long PrincipalPositionId { get; set; }

        public int ObjectBindingType { get; set; }
    }
}