using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Model.Aggregates;

using RulesetRule = NuClear.ValidationRules.Storage.Model.Facts.RulesetRule;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement RulesetRulesTests
        => ArrangeMetadataElement.Config
        .Name(nameof(RulesetRulesTests))
        .Fact(
            new RulesetRule { Id = 1, Priority = 1 },
            new RulesetRule { Id = 2, PrincipalPositionId = 1, DependentPositionId = 2, ObjectBindingType = 3, Priority = 2 }
            )
        .Aggregate(
            new Storage.Model.Aggregates.RulesetRule { RulesetId = 2, PrincipalPositionId = 1, DependentPositionId = 2, ObjectBindingType = 3 },
            new Ruleset { Id = 2 }
            );
    }
}
