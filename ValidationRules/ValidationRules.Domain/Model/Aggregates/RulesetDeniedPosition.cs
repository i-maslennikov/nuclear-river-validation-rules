namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    public sealed class RulesetDeniedPosition
    {
        public long RulesetId { get; set; }

        public long DeniedPositionId { get; set; }
        public long PrincipalPositionId { get; set; }

        public int ObjectBindingType { get; set; }
    }
}