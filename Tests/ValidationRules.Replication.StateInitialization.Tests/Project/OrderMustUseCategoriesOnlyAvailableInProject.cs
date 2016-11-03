using System;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;

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
                    new Facts::Order { Id = 1, Number = "Order", BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(3) },
                    new Facts::OrderPosition { Id = 1, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 1, CategoryId = 12, PositionId = 4 },
                    new Facts::Position { Id = 4, Name = "Position" },
                    new Facts::Category { Id = 12, Name = "Category" },
                    new Facts::Project())
                .Aggregate(
                    new Aggregates::Order { Id = 1, Number = "Order", Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.CategoryAdvertisement { OrderId = 1, OrderPositionId = 1, PositionId = 4, CategoryId = 12 },
                    new Aggregates::Position { Id = 4, Name = "Position" },
                    new Aggregates::Category { Id = 12, Name = "Category" })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse(
                                "<root><order id=\"1\" number=\"Order\" /><orderPosition id=\"1\" name=\"Position\" /><category id=\"12\" name=\"Category\" /></root>"),
                            MessageType = (int)MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject,
                            Result = 243,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(3),
                            OrderId = 1,
                        });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderMustUseCategoriesOnlyAvailableInProjectNegative
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustUseCategoriesOnlyAvailableInProjectNegative))
                .Fact(
                    new Facts::Order { Id = 1, Number = "Order", BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(3) },
                    new Facts::OrderPosition { Id = 1, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 1, CategoryId = 12, PositionId = 4 },
                    new Facts::Position { Id = 4, Name = "Position" },
                    new Facts::Category { Id = 12, Name = "Category" },
                    new Facts::Project(),
                    new Facts::CategoryOrganizationUnit { CategoryId = 12 })
                .Aggregate(
                    new Aggregates::Order { Id = 1, Number = "Order", Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.CategoryAdvertisement { OrderId = 1, OrderPositionId = 1, PositionId = 4, CategoryId = 12 },
                    new Aggregates::Position { Id = 4, Name = "Position" },
                    new Aggregates::Category { Id = 12, Name = "Category" },
                    new Aggregates::Project.Category { CategoryId = 12 })
                .Message();
    }
}
