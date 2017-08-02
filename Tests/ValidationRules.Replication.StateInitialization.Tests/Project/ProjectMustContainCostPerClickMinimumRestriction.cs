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
        private static ArrangeMetadataElement ProjectMustContainCostPerClickMinimumRestriction
            => ArrangeMetadataElement
                .Config
                .Name(nameof(ProjectMustContainCostPerClickMinimumRestriction))
                .Fact(
                    // Действует ограничение по 13-й рубрике (месяц 1)
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(3) },
                    new Facts::OrderPosition { Id = 1, OrderId = 1, PricePositionId = 5 },
                    new Facts::OrderPositionCostPerClick { OrderPositionId = 1, CategoryId = 13, Amount = 2 },

                    // Ограничение по 14-й рубрике обнуляет таковое по 13-й (месяц 2)
                    new Facts::Order { Id = 2, BeginDistribution = MonthStart(2), EndDistributionPlan = MonthStart(3) },
                    new Facts::OrderPosition { Id = 2, OrderId = 2, PricePositionId = 5 },
                    new Facts::OrderPositionCostPerClick { OrderPositionId = 2, CategoryId = 13, Amount = 2 },

                    new Facts::Position { Id = 4 },
                    new Facts::PricePosition { Id = 5, PositionId = 4 },

                    new Facts::Category { Id = 12, IsActiveNotDeleted = true },
                    new Facts::Category { Id = 13, IsActiveNotDeleted = true },

                    new Facts::Project(),
                    new Facts::CostPerClickCategoryRestriction { Begin = MonthStart(1), CategoryId = 13, MinCostPerClick = 2 },
                    new Facts::CostPerClickCategoryRestriction { Begin = MonthStart(2), CategoryId = 14, MinCostPerClick = 2 })
                .Aggregate(
                    new Aggregates::Order { Id = 1, Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.CostPerClickAdvertisement { OrderId = 1, OrderPositionId = 1, PositionId = 4, CategoryId = 13, Bid = 2 },

                    new Aggregates::Order { Id = 2, Begin = MonthStart(2), End = MonthStart(3) },
                    new Aggregates::Order.CostPerClickAdvertisement { OrderId = 2, OrderPositionId = 2, PositionId = 4, CategoryId = 13, Bid = 2 },

                    new Aggregates::Project(),
                    new Aggregates::Project.CostPerClickRestriction { CategoryId = 13, Begin = MonthStart(1), End = MonthStart(2), Minimum = 2 },
                    new Aggregates::Project.CostPerClickRestriction { CategoryId = 14, Begin = MonthStart(2), End = DateTime.MaxValue, Minimum = 2 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                                new Reference<EntityTypeCategory>(13),
                                                new Reference<EntityTypeProject>(0))
                                    .ToXDocument(),
                            MessageType = (int)MessageTypeCode.ProjectMustContainCostPerClickMinimumRestriction,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                            OrderId = 2,
                        }
                );
    }
}
