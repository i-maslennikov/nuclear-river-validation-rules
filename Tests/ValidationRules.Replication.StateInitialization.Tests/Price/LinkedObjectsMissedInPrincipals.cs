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
        private static ArrangeMetadataElement LinkedObjectsMissedInPrincipals
            => ArrangeMetadataElement
                .Config
                .Name(nameof(LinkedObjectsMissedInPrincipals))
                .Aggregate(
                    // Одобренный заказ с основной позицией
                    new Aggregates::Order { Id = 1, FirmId = 1 },
                    new Aggregates::Period.OrderPeriod { OrderId = 1, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::Order.OrderPosition { OrderId = 1, ItemPositionId = 1, Category1Id = 1, Category3Id = 3 },

                    // Заказ "на оформлении", с сопутствующей позицией, но с "неправильными" объектами привязки
                    new Aggregates::Order { Id = 2, FirmId = 1 },
                    new Aggregates::Period.OrderPeriod { OrderId = 2, Start = MonthStart(1), Scope = 2 },
                    new Aggregates::Order.OrderAssociatedPosition { OrderId = 2, CauseOrderPositionId = 3, CausePackagePositionId = 4, CauseItemPositionId = 4, PrincipalPositionId = 1, BindingType = 1, HasNoBinding = true },

                    new Aggregates::Position { Id = 1 },
                    new Aggregates::Position { Id = 4 },

                    new Aggregates::Period { Start = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Period.PricePeriod { Start = MonthStart(1) })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                        new Reference<EntityTypeOrderPosition>(3,
                                            new Reference<EntityTypeOrder>(2),
                                            new Reference<EntityTypePosition>(4),
                                            new Reference<EntityTypePosition>(4)))
                                    .ToXDocument(),

                            MessageType = (int)MessageTypeCode.LinkedObjectsMissedInPrincipals,
                            Result = 255,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 2,
                        });
    }
}
