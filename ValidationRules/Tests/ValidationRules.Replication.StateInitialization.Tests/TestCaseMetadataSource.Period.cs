using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    using Erm = NuClear.ValidationRules.Domain.Model.Erm;
    using Facts = NuClear.ValidationRules.Domain.Model.Facts;
    using Aggregates = NuClear.ValidationRules.Domain.Model.Aggregates;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement Demo
            => ArrangeMetadataElement.Config
                .Name(nameof(Demo))
                .Fact(
                    new Facts::Order { Id = 4 })
                .Erm(
                    new Erm::Order { Id = 4, IsActive = true, IsDeleted = false });
    }
}
