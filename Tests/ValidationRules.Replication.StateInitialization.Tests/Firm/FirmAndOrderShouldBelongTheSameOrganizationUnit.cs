using System;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.FirmRules.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmAndOrderShouldBelongTheSameOrganizationUnit
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmAndOrderShouldBelongTheSameOrganizationUnit))
                .Fact(
                    new Facts::Project { Id = 1, OrganizationUnitId = 2 },
                    new Facts::Project { Id = 2, OrganizationUnitId = 1 },
                    new Facts::Firm { Id = 1, OrganizationUnitId = 1, Name = "Firm" },
                    new Facts::Order { Id = 2, FirmId = 1, DestOrganizationUnitId = 2, Number = "InvalidOrder", BeginDistribution = FirstDayJan, EndDistributionFact = FirstDayFeb, WorkflowStep = 5 })
                .Aggregate(
                    new Aggregates::Firm { Id = 1, Name = "Firm", ProjectId = 2 },
                    new Aggregates::Order { Id = 2, FirmId = 1, Number = "InvalidOrder", Begin = FirstDayJan, End = FirstDayFeb, ProjectId = 1 },
                    new Aggregates::Order.FirmOrganiationUnitMismatch { OrderId = 2 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root><firm id = \"1\" name=\"Firm\" /><order id = \"2\" number=\"InvalidOrder\" /></root>"),
                            MessageType = (int)MessageTypeCode.FirmAndOrderShouldBelongTheSameOrganizationUnit,
                            Result = 255,
                            PeriodStart = FirstDayJan,
                            PeriodEnd = FirstDayFeb,
                            OrderId = 2,
                        });
    }
}

