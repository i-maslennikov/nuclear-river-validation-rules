using System;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionCostPerClickMustNotBeLessMinimum
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPositionCostPerClickMustNotBeLessMinimum))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(3) },
                    new Facts::OrderPosition { Id = 1, OrderId = 1, PricePositionId = 5 },
                    new Facts::OrderPositionCostPerClick { OrderPositionId = 1, CategoryId = 12, Amount = 1 },
                    new Facts::Position { Id = 4 },
                    new Facts::PricePosition { Id = 5, PositionId = 4 },
                    new Facts::Category { Id = 12, IsActiveNotDeleted = true },
                    new Facts::Project(),
                    new Facts::CostPerClickCategoryRestriction { Begin = MonthStart(1), CategoryId = 12, MinCostPerClick = 1 },
                    new Facts::CostPerClickCategoryRestriction { Begin = MonthStart(2), CategoryId = 12, MinCostPerClick = 2 })
                .Aggregate(
                    new Aggregates::Order { Id = 1, Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.CostPerClickAdvertisement { OrderId = 1, OrderPositionId = 1, PositionId = 4, CategoryId = 12, Bid = 1 },
                    new Aggregates::Project(),
                    new Aggregates::Project.CostPerClickRestriction { CategoryId = 12, Begin = MonthStart(1), End = MonthStart(2), Minimum = 1 },
                    new Aggregates::Project.CostPerClickRestriction { CategoryId = 12, Begin = MonthStart(2), End = DateTime.MaxValue, Minimum = 2 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                    new Reference<EntityTypeCategory>(12),
                                    new Reference<EntityTypeOrderPosition>(1,
                                        new Reference<EntityTypeOrder>(1),
                                        new Reference<EntityTypePosition>(4))).ToXDocument(),
                            MessageType = (int)MessageTypeCode.OrderPositionCostPerClickMustNotBeLessMinimum,
                            Result = 1023,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                            OrderId = 1,
                        });
    }
}
