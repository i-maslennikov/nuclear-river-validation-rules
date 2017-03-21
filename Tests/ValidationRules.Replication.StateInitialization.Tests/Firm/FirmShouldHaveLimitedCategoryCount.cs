using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

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
                    new Facts::Order { Id = 1, WorkflowStep = 5 },
                    new Facts::OrderPosition { Id = 4, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { OrderPositionId = 4, CategoryId = 5 })
                .Aggregate(
                    new Aggregates::Order { Id = 1 },
                    new Aggregates::Order.CategoryPurchase { OrderId = 1, CategoryId = 5 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmShouldHaveLimitedCategoryCountWhenNonIntersectingPeriods
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmShouldHaveLimitedCategoryCountWhenNonIntersectingPeriods))
                .Aggregate(
                    new Aggregates::Firm { Id = 1, },
                    new Aggregates::Order { Id = 1, FirmId = 1, Begin = FirstDayJan, End = FirstDayFeb },
                    new Aggregates::Order { Id = 2, FirmId = 1, Begin = FirstDayFeb, End = FirstDayApr })
                .Aggregate(
                    Enumerable.Range(1, 30).Select(i => new Aggregates::Order.CategoryPurchase { OrderId = 1, CategoryId = i }).ToArray()
                    )
                .Aggregate(
                    Enumerable.Range(1, 15).Select(i => new Aggregates::Order.CategoryPurchase { OrderId = 2, CategoryId = i }).ToArray()
                    )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                    new Dictionary<string, object> { { "count", 30 }, { "allowed", 20 } },
                                    new Reference<EntityTypeFirm>(1)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.FirmShouldHaveLimitedCategoryCount,
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
                    new Aggregates::Firm { Id = 1 },
                    new Aggregates::Order { Id = 1, FirmId = 1, Begin = FirstDayJan, End = FirstDayApr },
                    new Aggregates::Order { Id = 2, FirmId = 1, Begin = FirstDayFeb, End = FirstDayMay })
                .Aggregate(
                    Enumerable.Range(1, 15).Select(i => new Aggregates::Order.CategoryPurchase { OrderId = 1, CategoryId = i }).ToArray()
                    )
                .Aggregate(
                    Enumerable.Range(13, 15).Select(i => new Aggregates::Order.CategoryPurchase { OrderId = 2, CategoryId = i }).ToArray()
                    )
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                    new Dictionary<string, object> { { "count", 27 }, { "allowed", 20 } },
                                    new Reference<EntityTypeFirm>(1)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.FirmShouldHaveLimitedCategoryCount,
                        PeriodStart = FirstDayFeb,
                        PeriodEnd = FirstDayApr,
                        OrderId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                    new Dictionary<string, object> { { "count", 27 }, { "allowed", 20 } },
                                    new Reference<EntityTypeFirm>(1)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.FirmShouldHaveLimitedCategoryCount,
                        PeriodStart = FirstDayFeb,
                        PeriodEnd = FirstDayApr,
                        OrderId = 2,
                    });
    }
}
