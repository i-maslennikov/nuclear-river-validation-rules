using System;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderMustNotIncludeReleasedPeriodPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustNotIncludeReleasedPeriodPositive))
                .Fact(
                    new Facts::Order { Id = 1, Number = "Order", BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(3), WorkflowStep = 1 },
                    new Facts::ReleaseInfo { PeriodEndDate = MonthStart(2) },
                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, Number = "Order", Begin = MonthStart(1), End = MonthStart(3), IsDraft = true },
                    new Aggregates::Project.NextRelease { Date = MonthStart(2) })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse(
                                "<root><order id=\"1\" name=\"Order\" /></root>"),
                            MessageType = (int)MessageTypeCode.OrderMustNotIncludeReleasedPeriod,
                            Result = 3,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(3),
                            OrderId = 1,
                        });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderMustNotIncludeReleasedPeriodNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustNotIncludeReleasedPeriodNegative))
                .Fact(
                    new Facts::Order { Id = 1, Number = "Order", BeginDistribution = MonthStart(2), EndDistributionPlan = MonthStart(3), WorkflowStep = 1 },
                    new Facts::Order { Id = 2, Number = "Order", BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(3), WorkflowStep = 5 },
                    new Facts::ReleaseInfo { PeriodEndDate = MonthStart(2) },
                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, Number = "Order", Begin = MonthStart(2), End = MonthStart(3), IsDraft = true },
                    new Aggregates::Order { Id = 2, Number = "Order", Begin = MonthStart(1), End = MonthStart(3), IsDraft = false },
                    new Aggregates::Project.NextRelease { Date = MonthStart(2) })
                .Message();
    }
}
