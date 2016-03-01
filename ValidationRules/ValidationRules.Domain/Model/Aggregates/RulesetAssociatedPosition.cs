namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    public sealed class RulesetAssociatedPosition
    {
        public long RulesetId { get; set; }

        public long AssociatedPositionId { get; set; }
        public long PrincipalPositionId { get; set; }

        public int ObjectBindingType { get; set; }
    }
}