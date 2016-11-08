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
        private static ArrangeMetadataElement OrderPositionCostPerClickMustBeSpecifiedPositive
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPositionCostPerClickMustBeSpecifiedPositive))
                .Fact(
                    // Заказ с позицией с покликовой моделью, но без ставки - есть ошибка
                    new Facts::Order { Id = 1, Number = "Order", BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(3) },
                    new Facts::OrderPosition { Id = 1, OrderId = 1, PricePositionId = 5 },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 1, CategoryId = 12, PositionId = 4 },

                    // Заказ с позицией с обычной моделью - нет ошибки
                    new Facts::Order { Id = 2, Number = "Order", BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(3) },
                    new Facts::OrderPosition { Id = 2, OrderId = 2, PricePositionId = 5 },
                    new Facts::OrderPositionAdvertisement { Id = 2, OrderPositionId = 2, CategoryId = 12, PositionId = 5 },

                    // Заказ с позицией с покликовой моделью, со ставкой - нет ошибки
                    new Facts::Order { Id = 3, Number = "Order", BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(3) },
                    new Facts::OrderPosition { Id = 3, OrderId = 3, PricePositionId = 4 },
                    new Facts::OrderPositionAdvertisement { Id = 3, OrderPositionId = 3, CategoryId = 12, PositionId = 4 },
                    new Facts::OrderPositionCostPerClick { OrderPositionId = 3, CategoryId = 12 },

                    new Facts::PricePosition { Id = 4, PositionId = 4 },
                    new Facts::PricePosition { Id = 5, PositionId = 5 },
                    new Facts::Position { Id = 4, Name = "Position", SalesModel = 12 },
                    new Facts::Position { Id = 5, Name = "Position", SalesModel = 11 },
                    new Facts::Category { Id = 12, Name = "Category" },
                    new Facts::CategoryOrganizationUnit { CategoryId = 12 },
                    new Facts::Project())
                .Aggregate(
                    // Заказ с позицией с покликовой моделью, но без ставки - есть ошибка
                    new Aggregates::Order { Id = 1, Number = "Order", Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.CategoryAdvertisement { OrderId = 1, OrderPositionId = 1, PositionId = 4, CategoryId = 12, SalesModel = 12 },

                    // Заказ с позицией с обычной моделью - нет ошибки
                    new Aggregates::Order { Id = 2, Number = "Order", Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.CategoryAdvertisement { OrderId = 2, OrderPositionId = 2, PositionId = 5, CategoryId = 12, SalesModel = 11 },

                    // Заказ с позицией с покликовой моделью, со ставкой - нет ошибки
                    new Aggregates::Order { Id = 3, Number = "Order", Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.CategoryAdvertisement { OrderId = 3, OrderPositionId = 3, PositionId = 4, CategoryId = 12, SalesModel = 12 },
                    new Aggregates::Order.CostPerClickAdvertisement { OrderId = 3, OrderPositionId = 3, PositionId = 4, CategoryId = 12 },

                    new Aggregates::Category { Id = 12, Name = "Category" },
                    new Aggregates::Position { Id = 4, Name = "Position" },
                    new Aggregates::Position { Id = 5, Name = "Position" },
                    new Aggregates::Project.Category {CategoryId = 12})
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                XDocument.Parse(
                                    "<root><category id=\"12\" name=\"Category\" /><orderPosition id=\"1\" name=\"Position\" /><order id=\"1\" number=\"Order\" /></root>"),
                            MessageType = (int)MessageTypeCode.OrderPositionCostPerClickMustBeSpecified,
                            Result = 255,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(3),
                            OrderId = 1,
                        });
    }
}
