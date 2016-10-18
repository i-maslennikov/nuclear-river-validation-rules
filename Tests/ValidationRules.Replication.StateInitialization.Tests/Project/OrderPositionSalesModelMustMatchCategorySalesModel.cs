using System;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

using Aggregates = NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        private const int PositionsGroupMedia = 1;
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionSalesModelMustMatchCategorySalesModel
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPositionSalesModelMustMatchCategorySalesModel))
                .Fact(
                    // Заказ 1 с неправильным размещением в рубрике - есть ошибка
                    new Facts::Order { Id = 1, Number = "Order", BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(3) },
                    new Facts::OrderPosition { Id = 1, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 1, OrderPositionId = 1, PositionId = 1, CategoryId = 12 },
                    new Facts::Position { Id = 1, Name = "Position", SalesModel = 2 },

                    // Заказ 2 с корректным размещением в рубрике - нет ошибки
                    new Facts::Order { Id = 2, Number = "Order", BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(3) },
                    new Facts::OrderPosition { Id = 2, OrderId = 2 },
                    new Facts::OrderPositionAdvertisement { Id = 2, OrderPositionId = 2, PositionId = 2, CategoryId = 12 },
                    new Facts::Position { Id = 2, Name = "Position", SalesModel = 1 },

                    // Заказ 3 с неправильным размещением в рубрике, но позиция игнорирует модель продаж - нет ошибки
                    new Facts::Order { Id = 3, Number = "Order", BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(3) },
                    new Facts::OrderPosition { Id = 3, OrderId = 3 },
                    new Facts::OrderPositionAdvertisement { Id = 3, OrderPositionId = 3, PositionId = 3, CategoryId = 12 },
                    new Facts::PricePosition { Id = 3, PositionId = 3 },
                    new Facts::Position { Id = 3, Name = "Position", SalesModel = 2, PositionsGroup = PositionsGroupMedia },

                    // Заказ 4 начинает размещение корректно, но в процессе меняется модель продаж - есть ошибка
                    new Facts::Order { Id = 4, Number = "Order", BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(8) },
                    new Facts::OrderPosition { Id = 4, OrderId = 4 },
                    new Facts::OrderPositionAdvertisement { Id = 4, OrderPositionId = 4, PositionId = 4, CategoryId = 12 },
                    new Facts::Position { Id = 4, Name = "Position", SalesModel = 1 },

                    // Рубрика 12 в проекте 0 допускает размещение с моделью продаж 1 до 4-го месяца и моделью 2 позднее
                    new Facts::Project { Name = "Project" },
                    new Facts::CategoryOrganizationUnit { CategoryId = 12 },
                    new Facts::Category { Id = 12, Name = "Category", Level = 3 },
                    new Facts::SalesModelCategoryRestriction { CategoryId = 12, Begin = MonthStart(1), SalesModel = 1 },
                    new Facts::SalesModelCategoryRestriction { CategoryId = 12, Begin = MonthStart(4), SalesModel = 2 })
                .Aggregate(
                    // Заказ 1 с неправильным размещением в рубрике - есть ошибка
                    new Aggregates::Order { Id = 1, Number = "Order", Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.CategoryAdvertisement { OrderId = 1, OrderPositionId = 1, PositionId = 1, CategoryId = 12, SalesModel = 2, IsSalesModelRestrictionApplicable = true },

                    // Заказ 2 с корректным размещением в рубрике - нет ошибки
                    new Aggregates::Order { Id = 2, Number = "Order", Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.CategoryAdvertisement { OrderId = 2, OrderPositionId = 2, PositionId = 2, CategoryId = 12, SalesModel = 1, IsSalesModelRestrictionApplicable = true },

                    // Заказ 3 с неправильным размещением в рубрике, но позиция игнорирует модель продаж - нет ошибки
                    new Aggregates::Order { Id = 3, Number = "Order", Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.CategoryAdvertisement { OrderId = 3, OrderPositionId = 3, PositionId = 3, CategoryId = 12, SalesModel = 2, IsSalesModelRestrictionApplicable = false },

                    // Заказ 4 начинает размещение корректно, но в процессе меняется модель продаж - есть ошибка
                    new Aggregates::Order { Id = 4, Number = "Order", Begin = MonthStart(1), End = MonthStart(8) },
                    new Aggregates::Order.CategoryAdvertisement { OrderId = 4, OrderPositionId = 4, PositionId = 4, CategoryId = 12, SalesModel = 1, IsSalesModelRestrictionApplicable = true },

                    new Aggregates::Project { Name = "Project" },
                    new Aggregates::Project.Category { CategoryId = 12 },
                    new Aggregates::Project.SalesModelRestriction { CategoryId = 12, Begin = MonthStart(1), End = MonthStart(4), SalesModel = 1 },
                    new Aggregates::Project.SalesModelRestriction { CategoryId = 12, Begin = MonthStart(4), End = DateTime.MaxValue, SalesModel = 2 },

                    new Aggregates::Category { Id = 12, Name = "Category" },
                    new Aggregates::Position { Id = 1, Name = "Position" },
                    new Aggregates::Position { Id = 2, Name = "Position" },
                    new Aggregates::Position { Id = 3, Name = "Position" },
                    new Aggregates::Position { Id = 4, Name = "Position" })
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse(
                                "<root><category id=\"12\" name=\"Category\" /><orderPosition id=\"1\" name=\"Position\" /><order id=\"1\" number=\"Order\" /><project id=\"0\" name=\"Project\" /></root>"),
                        MessageType = (int)MessageTypeCode.OrderPositionSalesModelMustMatchCategorySalesModel,
                        Result = 255,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(3),
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = XDocument.Parse(
                                "<root><category id=\"12\" name=\"Category\" /><orderPosition id=\"4\" name=\"Position\" /><order id=\"4\" number=\"Order\" /><project id=\"0\" name=\"Project\" /></root>"),
                        MessageType = (int)MessageTypeCode.OrderPositionSalesModelMustMatchCategorySalesModel,
                        Result = 255,
                        PeriodStart = MonthStart(4),
                        PeriodEnd = MonthStart(8),
                    });
    }
}
