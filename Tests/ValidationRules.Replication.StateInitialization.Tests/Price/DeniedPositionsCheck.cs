using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Aggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement DeniedPositionsCheck
            => ArrangeMetadataElement
                .Config
                .Name(nameof(DeniedPositionsCheck))
                .Aggregate(
                    // Фирма 1: "На утверждении" видит "Одобренный", но не наоборот
                    // заказ с основной позицией
                    new Aggregates::Order { Id = 1, Number = "Order", FirmId = 1 },
                    new Aggregates::Period.OrderPeriod { OrderId = 1, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::Order.OrderPosition { OrderId = 1, OrderPositionId = 1, PackagePositionId = 1, ItemPositionId = 1, HasNoBinding = true },
                    new Aggregates::Order.OrderDeniedPosition { OrderId = 1, BindingType = 2, CauseOrderPositionId = 1, CausePackagePositionId = 1, CauseItemPositionId = 1, DeniedPositionId = 1 },

                    // заказ с конфликтующей позицией
                    new Aggregates::Order { Id = 2, Number = "Order", FirmId = 1 },
                    new Aggregates::Period.OrderPeriod { OrderId = 2, Start = MonthStart(1), Scope = -1 },
                    new Aggregates::Order.OrderPosition { OrderId = 2, OrderPositionId = 2, PackagePositionId = 1, ItemPositionId = 1, HasNoBinding = true },
                    new Aggregates::Order.OrderDeniedPosition { OrderId = 2, BindingType = 2, CauseOrderPositionId = 2, CausePackagePositionId = 1, CauseItemPositionId = 1, DeniedPositionId = 1 },

                    // Фирма 2: "На оформлении" видит "На утверждении", но не наоборот
                    // заказ с основной позицией
                    new Aggregates::Order { Id = 3, Number = "Order", FirmId = 2 },
                    new Aggregates::Period.OrderPeriod { OrderId = 3, Start = MonthStart(1), Scope = -1 },
                    new Aggregates::Order.OrderPosition { OrderId = 3, OrderPositionId = 3, PackagePositionId = 1, ItemPositionId = 1, HasNoBinding = true },
                    new Aggregates::Order.OrderDeniedPosition { OrderId = 3, BindingType = 2, CauseOrderPositionId = 3, CausePackagePositionId = 1, CauseItemPositionId = 1, DeniedPositionId = 1 },

                    // "на утверждении" с конфликтующей позицией
                    new Aggregates::Order { Id = 4, Number = "Order", FirmId = 2 },
                    new Aggregates::Period.OrderPeriod { OrderId = 4, Start = MonthStart(1), Scope = 4 },
                    new Aggregates::Order.OrderPosition { OrderId = 4, OrderPositionId = 4, PackagePositionId = 1, ItemPositionId = 1, HasNoBinding = true },
                    new Aggregates::Order.OrderDeniedPosition { OrderId = 4, BindingType = 2, CauseOrderPositionId = 4, CausePackagePositionId = 1, CauseItemPositionId = 1, DeniedPositionId = 1 },

                    // Фирма 3: "Одобренный" видит "Одобренный"
                    // заказ с основной позицией
                    new Aggregates::Order { Id = 5, Number = "Order", FirmId = 3 },
                    new Aggregates::Period.OrderPeriod { OrderId = 5, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::Order.OrderPosition { OrderId = 5, OrderPositionId = 5, PackagePositionId = 1, ItemPositionId = 1, HasNoBinding = true },
                    new Aggregates::Order.OrderDeniedPosition { OrderId = 5, BindingType = 2, CauseOrderPositionId = 5, CausePackagePositionId = 1, CauseItemPositionId = 1, DeniedPositionId = 1 },

                    // заказ с конфликтующей позицией
                    new Aggregates::Order { Id = 6, Number = "Order", FirmId = 3 },
                    new Aggregates::Period.OrderPeriod { OrderId = 6, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::Order.OrderPosition { OrderId = 6, OrderPositionId = 6, PackagePositionId = 1, ItemPositionId = 1, HasNoBinding = true },
                    new Aggregates::Order.OrderDeniedPosition { OrderId = 6, BindingType = 2, CauseOrderPositionId = 6, CausePackagePositionId = 1, CauseItemPositionId = 1, DeniedPositionId = 1 },

                    // Фирма 4: "На утверждении" видит "На утверждении"
                    // заказ с основной позицией
                    new Aggregates::Order { Id = 7, Number = "Order", FirmId = 4 },
                    new Aggregates::Period.OrderPeriod { OrderId = 7, Start = MonthStart(1), Scope = -1 },
                    new Aggregates::Order.OrderPosition { OrderId = 7, OrderPositionId = 7, PackagePositionId = 1, ItemPositionId = 1, HasNoBinding = true },
                    new Aggregates::Order.OrderDeniedPosition { OrderId = 7, BindingType = 2, CauseOrderPositionId = 7, CausePackagePositionId = 1, CauseItemPositionId = 1, DeniedPositionId = 1 },

                    // заказ с конфликтующей позицией
                    new Aggregates::Order { Id = 8, Number = "Order", FirmId = 4 },
                    new Aggregates::Period.OrderPeriod { OrderId = 8, Start = MonthStart(1), Scope = -1 },
                    new Aggregates::Order.OrderPosition { OrderId = 8, OrderPositionId = 8, PackagePositionId = 1, ItemPositionId = 1, HasNoBinding = true },
                    new Aggregates::Order.OrderDeniedPosition { OrderId = 8, BindingType = 2, CauseOrderPositionId = 8, CausePackagePositionId = 1, CauseItemPositionId = 1, DeniedPositionId = 1 },

                    new Aggregates::Position { Id = 1, Name = "Position" },

                    new Aggregates::Period { Start = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Period.PricePeriod { Start = MonthStart(1) })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<firm id=\"1\" />" +
                                                            "<position orderId=\"2\" orderNumber=\"Order\" orderPositionId=\"2\" orderPositionName=\"Position\" positionId=\"1\" positionName=\"Position\" />" +
                                                            "<position orderId=\"1\" orderNumber=\"Order\" orderPositionId=\"1\" orderPositionName=\"Position\" positionId=\"1\" positionName=\"Position\" />" +
                                                            "<order id=\"2\" name=\"Order\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.DeniedPositionsCheck,
                            Result = 255,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 2,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<firm id=\"2\" />" +
                                                            "<position orderId=\"4\" orderNumber=\"Order\" orderPositionId=\"4\" orderPositionName=\"Position\" positionId=\"1\" positionName=\"Position\" />" +
                                                            "<position orderId=\"3\" orderNumber=\"Order\" orderPositionId=\"3\" orderPositionName=\"Position\" positionId=\"1\" positionName=\"Position\" />" +
                                                            "<order id=\"4\" name=\"Order\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.DeniedPositionsCheck,
                            Result = 255,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 4,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<firm id=\"3\" />" +
                                                            "<position orderId=\"6\" orderNumber=\"Order\" orderPositionId=\"6\" orderPositionName=\"Position\" positionId=\"1\" positionName=\"Position\" />" +
                                                            "<position orderId=\"5\" orderNumber=\"Order\" orderPositionId=\"5\" orderPositionName=\"Position\" positionId=\"1\" positionName=\"Position\" />" +
                                                            "<order id=\"6\" name=\"Order\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.DeniedPositionsCheck,
                            Result = 255,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 6,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<firm id=\"3\" />" +
                                                            "<position orderId=\"5\" orderNumber=\"Order\" orderPositionId=\"5\" orderPositionName=\"Position\" positionId=\"1\" positionName=\"Position\" />" +
                                                            "<position orderId=\"6\" orderNumber=\"Order\" orderPositionId=\"6\" orderPositionName=\"Position\" positionId=\"1\" positionName=\"Position\" />" +
                                                            "<order id=\"5\" name=\"Order\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.DeniedPositionsCheck,
                            Result = 255,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 5,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<firm id=\"4\" />" +
                                                            "<position orderId=\"8\" orderNumber=\"Order\" orderPositionId=\"8\" orderPositionName=\"Position\" positionId=\"1\" positionName=\"Position\" />" +
                                                            "<position orderId=\"7\" orderNumber=\"Order\" orderPositionId=\"7\" orderPositionName=\"Position\" positionId=\"1\" positionName=\"Position\" />" +
                                                            "<order id=\"8\" name=\"Order\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.DeniedPositionsCheck,
                            Result = 255,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 8,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<firm id=\"4\" />" +
                                                            "<position orderId=\"7\" orderNumber=\"Order\" orderPositionId=\"7\" orderPositionName=\"Position\" positionId=\"1\" positionName=\"Position\" />" +
                                                            "<position orderId=\"8\" orderNumber=\"Order\" orderPositionId=\"8\" orderPositionName=\"Position\" positionId=\"1\" positionName=\"Position\" />" +
                                                            "<order id=\"7\" name=\"Order\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.DeniedPositionsCheck,
                            Result = 255,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 7,
                        });
    }
}
