using System;

using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    using Bit = NuClear.CustomerIntelligence.Domain.Model.Bit;
    using CI = NuClear.CustomerIntelligence.Domain.Model.CI;
    using Erm = NuClear.CustomerIntelligence.Domain.Model.Erm;
    using Facts = NuClear.CustomerIntelligence.Domain.Model.Facts;
    using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement CategoryGroupAggregate
            => ArrangeMetadataElement.Config
                                     .Name(nameof(CategoryGroupAggregate))
                                     .CustomerIntelligence(new CI::CategoryGroup { Id = 1, Name = "Name", Rate = 0.999f })
                                     .Fact(new Facts::CategoryGroup { Id = 1, Name = "Name", Rate = 0.999f });
    }
}
