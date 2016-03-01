using NuClear.River.Common.Metadata.Model;

namespace NuClear.ValidationRules.Domain.Model.Aggregates
{
    public sealed class Ruleset : IAggregateRoot
    {
        public long Id { get; set; }
    }
}