namespace NuClear.ValidationRules.Domain.Model.Facts
{
    public sealed class RulesetRule : IErmFactObject
    {
        public long Id { get; set; } // RulesetId
        public int RuleType { get; set; }
        public long DependentPositionId { get; set; }
        public long PrincipalPositionId { get; set; }
        public int Priority { get; set; }
        public int ObjectBindingType { get; set; }
    }
}
