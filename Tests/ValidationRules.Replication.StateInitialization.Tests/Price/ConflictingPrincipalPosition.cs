using System;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Facts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;
using Aggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ConflictingPrincipalPosition
            => ArrangeMetadataElement
                .Config
                .Name(nameof(ConflictingPrincipalPosition))
                .Aggregate(
                    // Фирма 1
                    // Одобренный заказ с основной позицией
                    new Aggregates::Order { Id = 1, Number = "Order", FirmId = 1 },
                    new Aggregates::OrderPeriod { OrderId = 1, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::OrderPeriod { OrderId = 1, Start = MonthStart(2), Scope = 0 },
                    new Aggregates::OrderPosition { OrderId = 1, OrderPositionId = 1, PackagePositionId = 1, ItemPositionId = 1, Category1Id = 1, Category3Id = 3 },

                    // Заказ "на утверждении" с сопутствующей позицией
                    new Aggregates::Order { Id = 2, Number = "Order", FirmId = 1 },
                    new Aggregates::OrderPeriod { OrderId = 2, Start = MonthStart(2), Scope = -1 },
                    new Aggregates::OrderAssociatedPosition { OrderId = 2, CauseOrderPositionId = 3, CausePackagePositionId = 4, CauseItemPositionId = 4, PrincipalPositionId = 1, BindingType = 3, Category1Id = 1, Category3Id = 3 },

                    // Фирма 2
                    // Одобренный заказ с основной позицией
                    new Aggregates::Order { Id = 3, Number = "Order", FirmId = 2 },
                    new Aggregates::OrderPeriod { OrderId = 3, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::OrderPeriod { OrderId = 3, Start = MonthStart(2), Scope = 0 },
                    new Aggregates::OrderPosition { OrderId = 3, OrderPositionId = 1, PackagePositionId = 1, ItemPositionId = 1, Category1Id = 1, Category3Id = 3 },

                    // Заказ "на оформлении" с сопутствующей позицией
                    new Aggregates::Order { Id = 4, Number = "Order", FirmId = 2 },
                    new Aggregates::OrderPeriod { OrderId = 4, Start = MonthStart(2), Scope = 4 },
                    new Aggregates::OrderAssociatedPosition { OrderId = 4, CauseOrderPositionId = 3, CausePackagePositionId = 4, CauseItemPositionId = 4, PrincipalPositionId = 1, BindingType = 3, Category1Id = 1, Category3Id = 3 },

                    // Фирма 3
                    // Заказ "на утверждении" с основной позицией
                    new Aggregates::Order { Id = 5, Number = "Order", FirmId = 3 },
                    new Aggregates::OrderPeriod { OrderId = 5, Start = MonthStart(1), Scope = -1 },
                    new Aggregates::OrderPeriod { OrderId = 5, Start = MonthStart(2), Scope = -1 },
                    new Aggregates::OrderPosition { OrderId = 5, OrderPositionId = 1, PackagePositionId = 1, ItemPositionId = 1, Category1Id = 1, Category3Id = 3 },

                    // Заказ "на оформлении", с сопутствующей позицией
                    new Aggregates::Order { Id = 6, Number = "Order", FirmId = 3 },
                    new Aggregates::OrderPeriod { OrderId = 6, Start = MonthStart(2), Scope = 6 },
                    new Aggregates::OrderAssociatedPosition { OrderId = 6, CauseOrderPositionId = 3, CausePackagePositionId = 4, CauseItemPositionId = 4, PrincipalPositionId = 1, BindingType = 3, Category1Id = 1, Category3Id = 3 },

                    new Aggregates::Position { Id = 1, Name = "Position" },
                    new Aggregates::Position { Id = 4, Name = "Position" },

                    new Aggregates::Period { Start = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Period { Start = MonthStart(2), End = MonthStart(3) },
                    new Aggregates::Period { Start = MonthStart(3), End = MonthStart(4) },
                    new Aggregates::Period { Start = MonthStart(4), End = MonthStart(5) },
                    new Aggregates::Period { Start = MonthStart(5), End = MonthStart(6) },
                    new Aggregates::PricePeriod { Start = MonthStart(1) })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<firm id=\"1\" />" +
                                                            "<position orderId=\"2\" orderNumber=\"Order\" orderPositionId=\"3\" orderPositionName=\"Position\" positionId=\"4\" positionName=\"Position\" />" +
                                                            "<position orderId=\"1\" orderNumber=\"Order\" orderPositionId=\"1\" orderPositionName=\"Position\" positionId=\"1\" positionName=\"Position\" />" +
                                                            "<order id=\"2\" name=\"Order\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.ConflictingPrincipalPosition,
                            Result = 255,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                            OrderId = 2,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<firm id=\"2\" />" +
                                                            "<position orderId=\"4\" orderNumber=\"Order\" orderPositionId=\"3\" orderPositionName=\"Position\" positionId=\"4\" positionName=\"Position\" />" +
                                                            "<position orderId=\"3\" orderNumber=\"Order\" orderPositionId=\"1\" orderPositionName=\"Position\" positionId=\"1\" positionName=\"Position\" />" +
                                                            "<order id=\"4\" name=\"Order\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.ConflictingPrincipalPosition,
                            Result = 255,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                            OrderId = 4,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<firm id=\"3\" />" +
                                                            "<position orderId=\"6\" orderNumber=\"Order\" orderPositionId=\"3\" orderPositionName=\"Position\" positionId=\"4\" positionName=\"Position\" />" +
                                                            "<position orderId=\"5\" orderNumber=\"Order\" orderPositionId=\"1\" orderPositionName=\"Position\" positionId=\"1\" positionName=\"Position\" />" +
                                                            "<order id=\"6\" name=\"Order\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.ConflictingPrincipalPosition,
                            Result = 255,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                            OrderId = 6,
                        });
    }
}
