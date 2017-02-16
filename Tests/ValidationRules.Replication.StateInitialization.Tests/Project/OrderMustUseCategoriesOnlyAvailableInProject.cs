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
        private static ArrangeMetadataElement OrderMustUseCategoriesOnlyAvailableInProjectPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustUseCategoriesOnlyAvailableInProjectPositive))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(3) },
                    new Facts::OrderPosition { Id = 1, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 1, CategoryId = 12, PositionId = 4 },
                    new Facts::Position { Id = 4 },
                    new Facts::Category { Id = 12, IsActiveNotDeleted = true },
                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.CategoryAdvertisement { OrderId = 1, OrderPositionId = 1, PositionId = 4, CategoryId = 12 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                        new Reference<EntityTypeOrder>(1),
                                        new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                            new Reference<EntityTypeOrderPosition>(1),
                                            new Reference<EntityTypePosition>(4)),
                                        new Reference<EntityTypeCategory>(12))
                                    .ToXDocument(),

                            MessageType = (int)MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject,
                            Result = 243,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(3),
                            OrderId = 1,
                        });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderMustUseCategoriesOnlyAvailableInProjectCategoryNotActive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustUseCategoriesOnlyAvailableInProjectCategoryNotActive))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(3) },
                    new Facts::OrderPosition { Id = 1, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 1, CategoryId = 12, PositionId = 4 },
                    new Facts::Position { Id = 4 },
                    // category not active
                    new Facts::Category { Id = 12, IsActiveNotDeleted = false },
                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, Begin = MonthStart(1), End = MonthStart(3) })
                .Message();

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderMustUseCategoriesOnlyAvailableInProjectNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustUseCategoriesOnlyAvailableInProjectNegative))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(3) },
                    new Facts::OrderPosition { Id = 1, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 1, CategoryId = 12, PositionId = 4 },
                    new Facts::Position { Id = 4 },
                    new Facts::Category { Id = 12, IsActiveNotDeleted = true },
                    new Facts::Project(),
                    new Facts::CategoryOrganizationUnit { CategoryId = 12 })
                .Aggregate(
                    new Aggregates::Order { Id = 1, Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.CategoryAdvertisement { OrderId = 1, OrderPositionId = 1, PositionId = 4, CategoryId = 12 },
                    new Aggregates::Project.Category { CategoryId = 12 })
                .Message();
    }
}
