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
        private static ArrangeMetadataElement OrderAssociatedPositionAccessorOpasByRuleSet
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderAssociatedPositionAccessorOpasByRuleSet))
                .Fact(
                    new Facts::Order { Id = 1 },
                    new Facts::OrderPosition { Id = 2, OrderId = 1, PricePositionId = 3 },
                    new Facts::PricePosition { Id = 3, PositionId = 4, IsActiveNotDeleted = true },
                    new Facts::Position { Id = 4, IsComposite = true },
                    new Facts::OrderPositionAdvertisement { Id = 5, OrderPositionId = 2, PositionId = 6 },

                    new Facts::RulesetRule { RuleType = 1, DependentPositionId = 6, PrincipalPositionId = 8 },
                    new Facts::RulesetRule { RuleType = 1, DependentPositionId = 4, PrincipalPositionId = 10 })
                .Aggregate(
                    new Aggregates::Order.OrderAssociatedPosition { OrderId = 1, CauseOrderPositionId = 2, CausePackagePositionId = 4, CauseItemPositionId = 6, PrincipalPositionId = 8, HasNoBinding = true, Source = Aggregates::PositionSources.Opa | Aggregates::PositionSources.Ruleset },
                    new Aggregates::Order.OrderAssociatedPosition { OrderId = 1, CauseOrderPositionId = 2, CausePackagePositionId = 4, CauseItemPositionId = 4, PrincipalPositionId = 10, HasNoBinding = true, Source = Aggregates::PositionSources.Pkg | Aggregates::PositionSources.Ruleset });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AssociatedPositionWithoutPrincipal
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AssociatedPositionWithoutPrincipal))
                .Aggregate(
                    // Одобренный заказ с основной позицией на три месяца
                    new Aggregates::Order { Id = 1, FirmId = 2 },
                    new Aggregates::Period.OrderPeriod { OrderId = 1, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::Period.OrderPeriod { OrderId = 1, Start = MonthStart(2), Scope = 0 },
                    new Aggregates::Period.OrderPeriod { OrderId = 1, Start = MonthStart(3), Scope = 0 },
                    new Aggregates::Order.OrderPosition { OrderId = 1, ItemPositionId = 1 },

                    // Заказ "на оформлении", с сопутствующей позицией, выходящей за период размещения основной
                    new Aggregates::Order { Id = 2, FirmId = 2 },
                    new Aggregates::Period.OrderPeriod { OrderId = 2, Start = MonthStart(2), Scope = 2 },
                    new Aggregates::Period.OrderPeriod { OrderId = 2, Start = MonthStart(3), Scope = 2 },
                    new Aggregates::Period.OrderPeriod { OrderId = 2, Start = MonthStart(4), Scope = 2 },
                    new Aggregates::Order.OrderAssociatedPosition { OrderId = 2, CauseOrderPositionId = 3, CausePackagePositionId = 4, CauseItemPositionId = 4, PrincipalPositionId = 1 },

                    new Aggregates::Position { Id = 2 },
                    new Aggregates::Position { Id = 4 },

                    new Aggregates::Period { Start = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Period { Start = MonthStart(2), End = MonthStart(3) },
                    new Aggregates::Period { Start = MonthStart(3), End = MonthStart(4) },
                    new Aggregates::Period { Start = MonthStart(4), End = MonthStart(5) },

                    new Aggregates::Period.PricePeriod { Start = MonthStart(1) })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<firm id=\"2\" />" +
                                                            "<orderPosition id=\"3\"><position id=\"4\" /></orderPosition>" +
                                                            "<opa><orderPosition id=\"3\" /><position id=\"4\" /></opa>" +
                                                            "<order id=\"2\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.AssociatedPositionWithoutPrincipal,
                            Result = 255,
                            PeriodStart = MonthStart(4),
                            PeriodEnd = MonthStart(5),
                            OrderId = 2,
                        });
    }
}
