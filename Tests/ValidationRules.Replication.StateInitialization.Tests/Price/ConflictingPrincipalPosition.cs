using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

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
        private static ArrangeMetadataElement ConflictingPrincipalPosition
            => ArrangeMetadataElement
                .Config
                .Name(nameof(ConflictingPrincipalPosition))
                .Aggregate(
                    // Фирма 1
                    // Одобренный заказ с основной позицией
                    new Aggregates::Order { Id = 1, FirmId = 1 },
                    new Aggregates::Period.OrderPeriod { OrderId = 1, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::Period.OrderPeriod { OrderId = 1, Start = MonthStart(2), Scope = 0 },
                    new Aggregates::Order.OrderPosition { OrderId = 1, OrderPositionId = 1, PackagePositionId = 1, ItemPositionId = 1, Category1Id = 1, Category3Id = 3 },

                    // Заказ "на утверждении" с сопутствующей позицией
                    new Aggregates::Order { Id = 2, FirmId = 1 },
                    new Aggregates::Period.OrderPeriod { OrderId = 2, Start = MonthStart(2), Scope = -1 },
                    new Aggregates::Order.OrderAssociatedPosition { OrderId = 2, CauseOrderPositionId = 3, CausePackagePositionId = 4, CauseItemPositionId = 4, PrincipalPositionId = 1, BindingType = 3, Category1Id = 1, Category3Id = 3 },

                    // Фирма 2
                    // Одобренный заказ с основной позицией
                    new Aggregates::Order { Id = 3, FirmId = 2 },
                    new Aggregates::Period.OrderPeriod { OrderId = 3, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::Period.OrderPeriod { OrderId = 3, Start = MonthStart(2), Scope = 0 },
                    new Aggregates::Order.OrderPosition { OrderId = 3, OrderPositionId = 1, PackagePositionId = 1, ItemPositionId = 1, Category1Id = 1, Category3Id = 3 },

                    // Заказ "на оформлении" с сопутствующей позицией
                    new Aggregates::Order { Id = 4, FirmId = 2 },
                    new Aggregates::Period.OrderPeriod { OrderId = 4, Start = MonthStart(2), Scope = 4 },
                    new Aggregates::Order.OrderAssociatedPosition { OrderId = 4, CauseOrderPositionId = 3, CausePackagePositionId = 4, CauseItemPositionId = 4, PrincipalPositionId = 1, BindingType = 3, Category1Id = 1, Category3Id = 3 },

                    // Фирма 3
                    // Заказ "на утверждении" с основной позицией
                    new Aggregates::Order { Id = 5, FirmId = 3 },
                    new Aggregates::Period.OrderPeriod { OrderId = 5, Start = MonthStart(1), Scope = -1 },
                    new Aggregates::Period.OrderPeriod { OrderId = 5, Start = MonthStart(2), Scope = -1 },
                    new Aggregates::Order.OrderPosition { OrderId = 5, OrderPositionId = 1, PackagePositionId = 1, ItemPositionId = 1, Category1Id = 1, Category3Id = 3 },

                    // Заказ "на оформлении", с сопутствующей позицией
                    new Aggregates::Order { Id = 6, FirmId = 3 },
                    new Aggregates::Period.OrderPeriod { OrderId = 6, Start = MonthStart(2), Scope = 6 },
                    new Aggregates::Order.OrderAssociatedPosition { OrderId = 6, CauseOrderPositionId = 3, CausePackagePositionId = 4, CauseItemPositionId = 4, PrincipalPositionId = 1, BindingType = 3, Category1Id = 1, Category3Id = 3 },

                    new Aggregates::Position { Id = 1 },
                    new Aggregates::Position { Id = 4 },

                    new Aggregates::Period { Start = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Period { Start = MonthStart(2), End = MonthStart(3) },
                    new Aggregates::Period { Start = MonthStart(3), End = MonthStart(4) },
                    new Aggregates::Period { Start = MonthStart(4), End = MonthStart(5) },
                    new Aggregates::Period { Start = MonthStart(5), End = MonthStart(6) },
                    new Aggregates::Period.PricePeriod { Start = MonthStart(1) })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                        new Reference<EntityTypeOrderPosition>(3,
                                            new Reference<EntityTypeOrder>(2),
                                            new Reference<EntityTypePosition>(4),
                                            new Reference<EntityTypePosition>(4)),

                                        new Reference<EntityTypeOrderPosition>(1,
                                            new Reference<EntityTypeOrder>(1),
                                            new Reference<EntityTypePosition>(1),
                                            new Reference<EntityTypePosition>(1)))
                                    .ToXDocument(),

                            MessageType = (int)MessageTypeCode.ConflictingPrincipalPosition,
                            Result = 13311,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                            OrderId = 2,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                        new Reference<EntityTypeOrderPosition>(3,
                                            new Reference<EntityTypeOrder>(4),
                                            new Reference<EntityTypePosition>(4),
                                            new Reference<EntityTypePosition>(4)),

                                        new Reference<EntityTypeOrderPosition>(1,
                                            new Reference<EntityTypeOrder>(3),
                                            new Reference<EntityTypePosition>(1),
                                            new Reference<EntityTypePosition>(1)))
                                    .ToXDocument(),

                            MessageType = (int)MessageTypeCode.ConflictingPrincipalPosition,
                            Result = 13311,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                            OrderId = 4,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                        new Reference<EntityTypeOrderPosition>(3,
                                            new Reference<EntityTypeOrder>(6),
                                            new Reference<EntityTypePosition>(4),
                                            new Reference<EntityTypePosition>(4)),

                                        new Reference<EntityTypeOrderPosition>(1,
                                            new Reference<EntityTypeOrder>(5),
                                            new Reference<EntityTypePosition>(1),
                                            new Reference<EntityTypePosition>(1)))
                                    .ToXDocument(),

                            MessageType = (int)MessageTypeCode.ConflictingPrincipalPosition,
                            Result = 13311,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                            OrderId = 6,
                        });
    }
}
