using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    public sealed class RulesetRule : IAggregateValueObject
    {
        public long RulesetId { get; set; }

        public int RuleType { get; set; }
        public long DependentPositionId { get; set; }
        public long PrincipalPositionId { get; set; }

        public int ObjectBindingType { get; set; }
    }
}