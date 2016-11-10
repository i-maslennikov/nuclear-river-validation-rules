using System;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Aggregates = NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.AccountRules.Facts;
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
                    new Facts::Order { Id = 1, Number = "InvalidOrder", BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 },
                    new Facts::Account { Id = 2, LegalPersonId = 1, BranchOfficeOrganizationUnitId = 1 },
                    new Facts::Lock { OrderId = 1, Start = FirstDayJan },
                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, Number = "InvalidOrder", BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar, AccountId = 2 },
                    new Aggregates::Lock { OrderId = 1, Start = FirstDayJan, End = FirstDayFeb },
                    new Aggregates::Account { Id = 2 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root><order id=\"1\" name=\"InvalidOrder\" /></root>"),
                            MessageType = (int)MessageTypeCode.LockShouldNotExist,
                            Result = 240,
                            PeriodStart = FirstDayJan,
                            PeriodEnd = FirstDayFeb,
                            OrderId = 1,
                        });
    }
}
