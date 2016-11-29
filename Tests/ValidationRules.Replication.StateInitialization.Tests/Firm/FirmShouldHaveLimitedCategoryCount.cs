using System.Linq;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmShouldHaveLimitedCategoryCount
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmShouldHaveLimitedCategoryCount))
                .Fact(
                    new Facts::Project { Id = 3, OrganizationUnitId = 2 },
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, WorkflowStep = 5 },
                    new Facts::OrderPosition { Id = 4, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, CategoryId = 5 })
                .Aggregate(
                    new Aggregates::Order { Id = 1, ProjectId = 3 },
                    new Aggregates::Order.CategoryPurchase { OrderId = 1, CategoryId = 5 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmShouldHaveLimitedCategoryCountWhenNonIntersectingPeriods
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmShouldHaveLimitedCategoryCountWhenNonIntersectingPeriods))
                .Aggregate(
                    new Aggregates::Firm { Id = 1, Name = "Firm" },
                    new Aggregates::Order { Id = 1, FirmId = 1, Number = "InvalidOrder", Begin = FirstDayJan, End = FirstDayFeb, ProjectId = 1 },
                    new Aggregates::Order { Id = 2, FirmId = 1, Number = "ValidOrder", Begin = FirstDayFeb, End = FirstDayApr, ProjectId = 1 })
                .Aggregate(
                    Enumerable.Range(1, 30).Select(i => new Aggregates::Order.CategoryPurchase { OrderId = 1, CategoryId = i }).ToArray()
                    )
                .Aggregate(
                    Enumerable.Range(1, 15).Select(i => new Aggregates::Order.CategoryPurchase { OrderId = 2, CategoryId = i }).ToArray()
                    )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><message count=\"30\" allowed=\"20\" /><firm id=\"1\" name=\"Firm\" /><order id=\"1\" name=\"InvalidOrder\" /></root>"),
                        MessageType = (int)MessageTypeCode.FirmShouldHaveLimitedCategoryCount,
                        Result = 42,
                        PeriodStart = FirstDayJan,
                        PeriodEnd = FirstDayFeb,
                        OrderId = 1,
                    });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmShouldHaveLimitedCategoryCountIntersectingPeriods
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmShouldHaveLimitedCategoryCountIntersectingPeriods))
                .Aggregate(
                    new Aggregates::Firm { Id = 1, Name = "Firm" },
                    new Aggregates::Order { Id = 1, FirmId = 1, Number = "InvalidOrder", Begin = FirstDayJan, End = FirstDayApr, ProjectId = 1 },
                    new Aggregates::Order { Id = 2, FirmId = 1, Number = "InvalidOrder", Begin = FirstDayFeb, End = FirstDayMay, ProjectId = 1 })
                .Aggregate(
                    Enumerable.Range(1, 15).Select(i => new Aggregates::Order.CategoryPurchase { OrderId = 1, CategoryId = i }).ToArray()
                    )
                .Aggregate(
                    Enumerable.Range(13, 15).Select(i => new Aggregates::Order.CategoryPurchase { OrderId = 2, CategoryId = i }).ToArray()
                    )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><message count=\"27\" allowed=\"20\" /><firm id=\"1\" name=\"Firm\" /><order id=\"1\" name=\"InvalidOrder\" /></root>"),
                        MessageType = (int)MessageTypeCode.FirmShouldHaveLimitedCategoryCount,
                        Result = 42,
                        PeriodStart = FirstDayFeb,
                        PeriodEnd = FirstDayApr,
                        OrderId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse("<root><message count=\"27\" allowed=\"20\" /><firm id=\"1\" name=\"Firm\" /><order id=\"2\" name=\"InvalidOrder\" /></root>"),
                        MessageType = (int)MessageTypeCode.FirmShouldHaveLimitedCategoryCount,
                        Result = 42,
                        PeriodStart = FirstDayFeb,
                        PeriodEnd = FirstDayApr,
                        OrderId = 2,
                    });
    }
}
