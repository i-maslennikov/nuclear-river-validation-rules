namespace NuClear.ValidationRules.Storage.Model.Erm
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
