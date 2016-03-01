namespace NuClear.ValidationRules.Domain.Model.Erm
{
    public sealed class GlobalDeniedPosition
    {
        public long Id { get; set; }
        public long DeniedPositionId { get; set; }
        public long PrincipalPositionId { get; set; }
        public int ObjectBindingType { get; set; }
        public long RulesetId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
