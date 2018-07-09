using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement LegalPersonProfileWarrantyShouldNotBeExpired
            => ArrangeMetadataElement
                .Config
                .Name(nameof(LegalPersonProfileWarrantyShouldNotBeExpired))
                .Fact(
                    new Facts::Order { Id = 1, LegalPersonId = 1, SignupDate = MonthStart(1), BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Facts::LegalPersonProfile { Id = 1, LegalPersonId = 1, WarrantyEndDate = MonthStart(1).AddDays(-1) })
                .Aggregate(
                    new Aggregates::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.LegalPersonProfileWarrantyExpired { OrderId = 1, LegalPersonProfileId = 1 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Reference<EntityTypeLegalPersonProfile>(1),
                                    new Reference<EntityTypeOrder>(1))
                                .ToXDocument(),
                            MessageType = (int)MessageTypeCode.LegalPersonProfileWarrantyShouldNotBeExpired,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        });
    }
}
