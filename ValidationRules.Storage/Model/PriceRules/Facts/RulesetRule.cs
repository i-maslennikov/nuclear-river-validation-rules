namespace NuClear.ValidationRules.Storage.Model.PriceRules.Facts
{
    public sealed class RulesetRule
    {
        public long Id { get; set; } // TODO: убрать хак, заменить на RulesetId
        public int RuleType { get; set; }
        public long DependentPositionId { get; set; }
        public long PrincipalPositionId { get; set; }
        public int Priority { get; set; }
        public int ObjectBindingType { get; set; }
    }
}
