using System;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.SystemRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AmsMessagesShouldBeNew
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AmsMessagesShouldBeNew))
                .Fact(
                    new Facts::SystemStatus { Id = 1, SystemIsDown = true }
                )
                .Aggregate(
                    new Aggregates::SystemStatus { Id = 1, SystemIsDown = true }
                )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams().ToXDocument(),
                            MessageType = (int)MessageTypeCode.AmsMessagesShouldBeNew,
                            PeriodStart = DateTime.MinValue,
                            PeriodEnd = DateTime.MaxValue,
                        }
                );
    }
}
