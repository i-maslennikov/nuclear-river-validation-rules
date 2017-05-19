using System;

using NuClear.DataTest.Metamodel.Dsl;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement Version
        => ArrangeMetadataElement.Config
            .Name(nameof(Version))
            .Aggregate()
            .Message(new Version(), new Version.ErmState())
            .Ignored();
    }
}
