using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    using Facts = Domain.Model.Facts;
    using Aggs = Domain.Model.Aggregates;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement RulesetDeniedPositionTests
        => ArrangeMetadataElement.Config
        .Name(nameof(RulesetDeniedPositionTests))
        .Fact(
            // denied
            new Facts::GlobalDeniedPosition { PrincipalPositionId = 1, DeniedPositionId = 2, ObjectBindingType = 3, RulesetId = 4, Id = 1}
            )
        .Aggregate(
            // denied
            new Aggs::RulesetDeniedPosition { PrincipalPositionId = 1, DeniedPositionId = 2, ObjectBindingType = 3, RulesetId = 4},
            new Aggs::Ruleset { Id = 4 }
            );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement RulesetAssociatedPositionTests
        => ArrangeMetadataElement.Config
        .Name(nameof(RulesetAssociatedPositionTests))
        .Fact(
            // associated
            new Facts::GlobalAssociatedPosition { PrincipalPositionId = 1, AssociatedPositionId = 2, ObjectBindingType = 3, RulesetId = 4, Id = 1 }
            )
        .Aggregate(
            // associated
            new Aggs::RulesetAssociatedPosition { PrincipalPositionId = 1, AssociatedPositionId = 2, ObjectBindingType = 3, RulesetId = 4 },
            new Aggs::Ruleset { Id = 4 }
            );
    }
}
