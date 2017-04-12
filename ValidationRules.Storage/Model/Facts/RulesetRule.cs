namespace NuClear.ValidationRules.Storage.Model.Facts
{
    public sealed class RulesetRule
    {
        public const int Associated = 1;
        public const int Denied = 2;

        public int RuleType { get; set; }
        public long DependentPositionId { get; set; }
        public long PrincipalPositionId { get; set; }
        public int ObjectBindingType { get; set; }
    }
}
