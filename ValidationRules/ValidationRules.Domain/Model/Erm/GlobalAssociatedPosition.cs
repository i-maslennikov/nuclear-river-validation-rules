namespace NuClear.ValidationRules.Domain.Model.Erm
{
    public sealed class GlobalAssociatedPosition
    {
        public long Id { get; set; }
        public long RulesetId { get; set; }
        public long AssociatedPositionId { get; set; }
        public long PrincipalPositionId { get; set; }
        public int ObjectBindingType { get; set; }
        public bool IsDeleted { get; set; }
    }
}
