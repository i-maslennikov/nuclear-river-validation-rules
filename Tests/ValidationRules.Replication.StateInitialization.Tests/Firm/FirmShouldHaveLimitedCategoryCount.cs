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
        private static ArrangeMetadataElement FirmShouldHaveLimitedCategoryCountAggregates
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmShouldHaveLimitedCategoryCountAggregates))
                .Fact(
                    new Facts::Order { Id = 1, WorkflowStep = 4, FirmId = 1, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(3), EndDistributionPlan = MonthStart(4) },
                    new Facts::OrderItem { OrderId = 1, CategoryId = 5 },

                    new Facts::Order { Id = 2, WorkflowStep = 5, FirmId = 1, BeginDistribution = MonthStart(3), EndDistributionFact = MonthStart(5), EndDistributionPlan = MonthStart(5) },
                    new Facts::OrderItem { OrderId = 2, CategoryId = 5 },
                    new Facts::OrderItem { OrderId = 2, CategoryId = 6 },

                    new Facts::Order { Id = 3, WorkflowStep = 1, FirmId = 2, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(5), EndDistributionPlan = MonthStart(5) },
                    new Facts::OrderItem { OrderId = 3, CategoryId = 5 },
                    new Facts::Order { Id = 4, WorkflowStep = 1, FirmId = 2, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(5), EndDistributionPlan = MonthStart(5) },
                    new Facts::OrderItem { OrderId = 4, CategoryId = 5 })
                .Aggregate(
                    // Периоды строятся по 
                    new Aggregates::Firm.CategoryPurchase { FirmId = 1, CategoryId = 5, Begin = MonthStart(1), End = MonthStart(3), Scope = 0 },
                    new Aggregates::Firm.CategoryPurchase { FirmId = 1, CategoryId = 5, Begin = MonthStart(3), End = MonthStart(4), Scope = 1 },
                    new Aggregates::Firm.CategoryPurchase { FirmId = 1, CategoryId = 5, Begin = MonthStart(3), End = MonthStart(4), Scope = 0 },
                    new Aggregates::Firm.CategoryPurchase { FirmId = 1, CategoryId = 5, Begin = MonthStart(4), End = MonthStart(5), Scope = 0 },
                    new Aggregates::Firm.CategoryPurchase { FirmId = 1, CategoryId = 6, Begin = MonthStart(3), End = MonthStart(4), Scope = 0 },
                    new Aggregates::Firm.CategoryPurchase { FirmId = 1, CategoryId = 6, Begin = MonthStart(4), End = MonthStart(5), Scope = 0 },

                    // Периоды для разных фирм не зависят друг от друга. Рубрики могут дубливаться в разных Scope.
                    new Aggregates::Firm.CategoryPurchase { FirmId = 2, CategoryId = 5, Begin = MonthStart(1), End = MonthStart(5), Scope = 3 },
                    new Aggregates::Firm.CategoryPurchase { FirmId = 2, CategoryId = 5, Begin = MonthStart(1), End = MonthStart(5), Scope = 4 });


        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmShouldHaveLimitedCategoryCountMessages
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmShouldHaveLimitedCategoryCountMessages))
                .Aggregate(
                    new Aggregates::Order { Id = 1, FirmId = 1, Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order { Id = 2, FirmId = 1, Begin = MonthStart(2), End = MonthStart(4) })
                .Aggregate(Enumerable.Range(1, 15).Select(i => new Aggregates::Firm.CategoryPurchase { FirmId = 1, CategoryId = i, Begin = MonthStart(1), End = MonthStart(2) }).ToArray())
                .Aggregate(Enumerable.Range(1, 27).Select(i => new Aggregates::Firm.CategoryPurchase { FirmId = 1, CategoryId = i, Begin = MonthStart(2), End = MonthStart(3) }).ToArray())
                .Aggregate(Enumerable.Range(13, 15).Select(i => new Aggregates::Firm.CategoryPurchase { FirmId = 1, CategoryId = i, Begin = MonthStart(3), End = MonthStart(4) }).ToArray())
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                    new Dictionary<string, object> { { "count", 27 }, { "allowed", 20 } },
                                    new Reference<EntityTypeFirm>(1)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.FirmShouldHaveLimitedCategoryCount,
                        PeriodStart = MonthStart(2),
                        PeriodEnd = MonthStart(3),
                        OrderId = 1,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(
                                    new Dictionary<string, object> { { "count", 27 }, { "allowed", 20 } },
                                    new Reference<EntityTypeFirm>(1)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.FirmShouldHaveLimitedCategoryCount,
                        PeriodStart = MonthStart(2),
                        PeriodEnd = MonthStart(3),
                        OrderId = 2,
                    });
    }
}
