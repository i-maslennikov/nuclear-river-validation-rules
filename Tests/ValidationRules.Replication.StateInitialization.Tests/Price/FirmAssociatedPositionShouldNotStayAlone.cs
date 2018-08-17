using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmAssociatedPositionShouldNotStayAlone
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmAssociatedPositionShouldNotStayAlone))
                .Aggregate(
                    // Когда основная позиция - ровно одна и находится в другом заказе, должно быть сообщение
                    new Aggregates::Firm { Id = 1 },
                    new Aggregates::Firm.FirmPosition { FirmId = 1, OrderId = 1, OrderPositionId = 1, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 2 },
                    new Aggregates::Firm.FirmPosition { FirmId = 1, OrderId = 2, OrderPositionId = 2, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 3, ItemPositionId = 3 },

                    new Aggregates::Firm.FirmAssociatedPosition { FirmId = 1, OrderPositionId = 1, BindingType = 2, PackagePositionId = 2, ItemPositionId = 2, PrincipalPositionId = 3 },

                    // Когда основных несколько в разных заказах - сообщения нет
                    new Aggregates::Firm { Id = 2 },
                    new Aggregates::Firm.FirmPosition { FirmId = 2, OrderId = 3, OrderPositionId = 3, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 2 },
                    new Aggregates::Firm.FirmPosition { FirmId = 2, OrderId = 4, OrderPositionId = 4, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 3, ItemPositionId = 3 },
                    new Aggregates::Firm.FirmPosition { FirmId = 2, OrderId = 5, OrderPositionId = 5, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 3, ItemPositionId = 3 },

                    new Aggregates::Firm.FirmAssociatedPosition { FirmId = 2, OrderPositionId = 3, BindingType = 2, PackagePositionId = 2, ItemPositionId = 2, PrincipalPositionId = 3 },

                    // Когда основных несколько в одном заказе заказе - сообщение есть
                    new Aggregates::Firm { Id = 3 },
                    new Aggregates::Firm.FirmPosition { FirmId = 3, OrderId = 6, OrderPositionId = 6, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 2 },
                    new Aggregates::Firm.FirmPosition { FirmId = 3, OrderId = 7, OrderPositionId = 7, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 3, ItemPositionId = 3 },
                    new Aggregates::Firm.FirmPosition { FirmId = 3, OrderId = 7, OrderPositionId = 8, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 3, ItemPositionId = 3 },

                    new Aggregates::Firm.FirmAssociatedPosition { FirmId = 3, OrderPositionId = 6, BindingType = 2, PackagePositionId = 2, ItemPositionId = 2, PrincipalPositionId = 3 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                        new Reference<EntityTypeOrderPosition>(2,
                                            new Reference<EntityTypeOrder>(2),
                                            new Reference<EntityTypePosition>(3),
                                            new Reference<EntityTypePosition>(3)),

                                        new Reference<EntityTypeOrderPosition>(1,
                                            new Reference<EntityTypeOrder>(1),
                                            new Reference<EntityTypePosition>(2),
                                            new Reference<EntityTypePosition>(2)))
                                    .ToXDocument(),

                            MessageType = (int)MessageTypeCode.FirmAssociatedPositionShouldNotStayAlone,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 2,
                        },

                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                        new Reference<EntityTypeOrderPosition>(7,
                                            new Reference<EntityTypeOrder>(7),
                                            new Reference<EntityTypePosition>(3),
                                            new Reference<EntityTypePosition>(3)),

                                        new Reference<EntityTypeOrderPosition>(6,
                                            new Reference<EntityTypeOrder>(6),
                                            new Reference<EntityTypePosition>(2),
                                            new Reference<EntityTypePosition>(2)))
                                    .ToXDocument(),

                            MessageType = (int)MessageTypeCode.FirmAssociatedPositionShouldNotStayAlone,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 7,
                        });
    }
}
