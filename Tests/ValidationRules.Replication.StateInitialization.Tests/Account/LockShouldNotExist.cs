using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement LockShouldNotExist
            => ArrangeMetadataElement
                .Config
                .Name(nameof(LockShouldNotExist))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayMar, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1, WorkflowStep = 4 },
                    new Facts::Account { Id = 2, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 },
                    new Facts::Lock { OrderId = 1, Start = FirstDayJan },
                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar, AccountId = 2 },
                    new Aggregates::Order.Lock { OrderId = 1, Start = FirstDayJan, End = FirstDayFeb },
                    new Aggregates::Account { Id = 2 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(new Reference<EntityTypeOrder>(1)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.LockShouldNotExist,
                            PeriodStart = FirstDayJan,
                            PeriodEnd = FirstDayFeb,
                            OrderId = 1,
                        });
    }
}
