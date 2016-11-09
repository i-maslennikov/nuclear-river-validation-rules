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
        private static ArrangeMetadataElement AccountShouldExistNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AccountShouldExistNegative))
                .Fact(
                    new Facts::Order { Id = 1, Number = "InvalidOrder", LegalPersonId = 2, BranchOfficeOrganizationUnitId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar },
                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, Number = "InvalidOrder", AccountId = null, BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root><order id=\"1\" number=\"InvalidOrder\" /></root>"),
                            MessageType = (int)MessageTypeCode.AccountShouldExist,
                            Result = 240,
                            PeriodStart = FirstDayJan,
                            PeriodEnd = FirstDayMar,
                            OrderId = 1,
                        });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AccountShouldExistPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AccountShouldExistPositive))
                .Fact(
                    new Facts::Order { Id = 1, Number = "ValidOrder", LegalPersonId = 2, BranchOfficeOrganizationUnitId = 3, BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar },
                    new Facts::Account { Id = 4, LegalPersonId = 2, BranchOfficeOrganizationUnitId = 3 },
                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, Number = "ValidOrder", AccountId = 4, BeginDistributionDate = FirstDayJan, EndDistributionDate = FirstDayMar })
                .Message();
    }
}
