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
            new Facts::GlobalDeniedPosition { RulesetId = 1, Id = 1 },
            new Facts::GlobalDeniedPosition { RulesetId = 2, PrincipalPositionId = 1, DeniedPositionId = 2, ObjectBindingType = 3, Id = 2}
            )
        .Aggregate(
            new Aggs::RulesetDeniedPosition { RulesetId = 2, PrincipalPositionId = 1, DeniedPositionId = 2, ObjectBindingType = 3},
            new Aggs::Ruleset { Id = 2 }
            );

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement RulesetAssociatedPositionTests
        => ArrangeMetadataElement.Config
        .Name(nameof(RulesetAssociatedPositionTests))
        .Fact(
            new Facts::GlobalAssociatedPosition { RulesetId = 1, Id = 1 },
            new Facts::GlobalAssociatedPosition { RulesetId = 2, PrincipalPositionId = 1, AssociatedPositionId = 2, ObjectBindingType = 3, Id = 2 }
            )
        .Aggregate(
            new Aggs::RulesetAssociatedPosition { RulesetId = 2, PrincipalPositionId = 1, AssociatedPositionId = 2, ObjectBindingType = 3 },
            new Aggs::Ruleset { Id = 2 }
            );
    }
}
