using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Utils.Xml;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositionsAggregate
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositionsAggregate))
                .Fact(
                    new Facts::Order { Id = 1 },
                    new Facts::OrderPosition { Id = 2, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 3, OrderPositionId = 2, PositionId = 4 },
                    new Facts::Position { Id = 4, CategoryCode = 287, Platform = 1},

                    new Facts::Order { Id = 5 },
                    new Facts::OrderPosition { Id = 6, OrderId = 5 },
                    new Facts::OrderPositionAdvertisement { Id = 7, OrderPositionId = 6, PositionId = 8 },
                    new Facts::Position { Id = 8, Platform = ~1 })
                .Aggregate(
                    new Aggregates::Order.SelfAdvertisementPosition { OrderId = 1 },
                    new Aggregates::Order.NotApplicapleForDesktopPosition { OrderId = 5 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositionsMessage
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositionsMessage))
                .Aggregate(
                    // Обе позиции в одном заказе - есть ошибка
                    new Aggregates::Order { Id = 1, FirmId = 1, Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.SelfAdvertisementPosition { OrderId = 1 },
                    new Aggregates::Order.NotApplicapleForDesktopPosition { OrderId = 1 },

                    // В разных заказах, с пересечением по времени размещения - есть ошибка в обоих заказах
                    new Aggregates::Order { Id = 2, FirmId = 2, Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.SelfAdvertisementPosition { OrderId = 2 },
                    new Aggregates::Order { Id = 3, FirmId = 2, Begin = MonthStart(2), End = MonthStart(4) },
                    new Aggregates::Order.NotApplicapleForDesktopPosition { OrderId = 3 },

                    // В разных заказах, без пересечения по времени размещения - нет ошибки
                    new Aggregates::Order { Id = 4, FirmId = 4, Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.SelfAdvertisementPosition { OrderId = 4 },
                    new Aggregates::Order { Id = 5, FirmId = 4, Begin = MonthStart(3), End = MonthStart(5) },
                    new Aggregates::Order.NotApplicapleForDesktopPosition { OrderId = 5 },

                    // В разных заказах в разных состояниях, с пересечением по времени размещения - есть ошибка, только в локальном
                    new Aggregates::Order { Id = 6, FirmId = 6, Begin = MonthStart(1), End = MonthStart(3), Scope = 6},
                    new Aggregates::Order.SelfAdvertisementPosition { OrderId = 6 },
                    new Aggregates::Order { Id = 7, FirmId = 6, Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.NotApplicapleForDesktopPosition { OrderId = 7 },

                    // В разных заказах в разных состояниях, с пересечением по времени размещения - есть ошибка, только в локальном
                    new Aggregates::Order { Id = 8, FirmId = 8, Begin = MonthStart(1), End = MonthStart(3) },
                    new Aggregates::Order.SelfAdvertisementPosition { OrderId = 8 },
                    new Aggregates::Order { Id = 9, FirmId = 8, Begin = MonthStart(1), End = MonthStart(3), Scope = 9 },
                    new Aggregates::Order.NotApplicapleForDesktopPosition { OrderId = 9 })
                .Message(
                    // Обе позиции в одном заказе - есть ошибка
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(new Reference<EntityTypeOrder>(1)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositions,
                        Result = 1023,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(3),
                        OrderId = 1,
                    },

                    // В разных заказах, с пересечением по времени размещения - есть ошибка в обоих заказах
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(new Reference<EntityTypeOrder>(2)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositions,
                        Result = 1023,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(3),
                        OrderId = 2,
                    },
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(new Reference<EntityTypeOrder>(3)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositions,
                        Result = 1023,
                        PeriodStart = MonthStart(2),
                        PeriodEnd = MonthStart(4),
                        OrderId = 3,
                    },

                    // В разных заказах в разных состояниях, с пересечением по времени размещения - есть ошибка, только в локальном
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(new Reference<EntityTypeOrder>(6)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositions,
                        Result = 1023,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(3),
                        OrderId = 6,
                    },

                    // В разных заказах в разных состояниях, с пересечением по времени размещения - есть ошибка, только в локальном
                    new Messages::Version.ValidationResult
                    {
                        MessageParams = new MessageParams(new Reference<EntityTypeOrder>(9)).ToXDocument(),
                        MessageType = (int)MessageTypeCode.FirmWithSelfAdvMustHaveOnlyDesktopOrIndependentPositions,
                        Result = 1023,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(3),
                        OrderId = 9,
                    });
    }
}
