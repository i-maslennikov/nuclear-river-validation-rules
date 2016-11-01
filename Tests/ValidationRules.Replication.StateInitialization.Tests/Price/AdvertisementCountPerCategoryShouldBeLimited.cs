using System;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;


using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Aggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementCountPerCategoryShouldBeLimited
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AdvertisementCountPerCategoryShouldBeLimited))
                .Aggregate(
                    // Одобренный заказ с продажей на три месяца
                    new Aggregates::Order { Id = 1, Number = "Order" },
                    new Aggregates::OrderPosition { OrderId = 1, ItemPositionId = 1, Category1Id = 1, Category3Id = 3 },
                    new Aggregates::OrderPeriod { OrderId = 1, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::OrderPeriod { OrderId = 1, Start = MonthStart(2), Scope = 0 },
                    new Aggregates::OrderPeriod { OrderId = 1, Start = MonthStart(3), Scope = 0 },

                    // Другой одобренный заказ с продажей (пересекается только в одном месяце)
                    new Aggregates::Order { Id = 2, Number = "Order" },
                    new Aggregates::OrderPosition { OrderId = 2, ItemPositionId = 1, Category1Id = 1, Category3Id = 3 },
                    new Aggregates::OrderPeriod { OrderId = 2, Start = MonthStart(3), Scope = 0 },
                    new Aggregates::OrderPeriod { OrderId = 2, Start = MonthStart(4), Scope = 0 },
                    new Aggregates::OrderPeriod { OrderId = 2, Start = MonthStart(5), Scope = 0 },

                    // Заказ "на утверждении", размещается, когда есть две одобренных продажи и получает ошибку
                    new Aggregates::Order { Id = 3, Number = "Order" },
                    new Aggregates::OrderPosition { OrderId = 3, ItemPositionId = 1, Category1Id = 1, Category3Id = 3 },
                    new Aggregates::OrderPeriod { OrderId = 3, Start = MonthStart(3), Scope = -1 },
                    new Aggregates::OrderPeriod { OrderId = 3, Start = MonthStart(4), Scope = -1 },

                    // Заказ "на оформлении", размещается, когда есть одна одобренная продажа и один заказ на оформлении и тоже получает ошибку
                    new Aggregates::Order { Id = 4, Number = "Order" },
                    new Aggregates::OrderPosition { OrderId = 4, ItemPositionId = 1, Category1Id = 1, Category3Id = 3 },
                    new Aggregates::OrderPeriod { OrderId = 4, Start = MonthStart(4), Scope = 4 },
                    new Aggregates::OrderPeriod { OrderId = 4, Start = MonthStart(5), Scope = 4 },

                    new Aggregates::Position { Id = 1, CategoryCode = 38, Name = "Position" },

                    new Aggregates::Category { Id = 3, Name = "Category" },

                    new Aggregates::Period { Start = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Period { Start = MonthStart(2), End = MonthStart(3) },
                    new Aggregates::Period { Start = MonthStart(3), End = MonthStart(4) },
                    new Aggregates::Period { Start = MonthStart(4), End = MonthStart(5) },
                    new Aggregates::Period { Start = MonthStart(5), End = MonthStart(6) },
                    new Aggregates::Period { Start = MonthStart(6), End = MonthStart(7) })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root><message max=\"2\" count=\"3\" /><category id=\"3\" name=\"Category\" /><order id=\"3\" number=\"Order\" /></root>"),
                            MessageType = (int)MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited,
                            Result = 254,
                            PeriodStart = MonthStart(3),
                            PeriodEnd = MonthStart(4),
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root><message max=\"2\" count=\"3\" /><category id=\"3\" name=\"Category\" /><order id=\"4\" number=\"Order\" /></root>"),
                            MessageType = (int)MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited,
                            Result = 254,
                            PeriodStart = MonthStart(4),
                            PeriodEnd = MonthStart(5),
                        });
    }
}
