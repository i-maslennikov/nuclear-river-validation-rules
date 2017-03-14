using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Aggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject))
                .Aggregate(
                    // Есть основная позиция, но с отличающимся объектом привязки - получаем сообщение о несовпадении объектов
                    new Aggregates::Firm { Id = 1 },
                    new Aggregates::Firm.FirmPosition { FirmId = 1, OrderPositionId = 1, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 2, Category1Id = 1, Category3Id = 2 },
                    new Aggregates::Firm.FirmPosition { FirmId = 1, OrderPositionId = 2, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 3, Category1Id = 1, Category3Id = 3 },

                    new Aggregates::Firm.FirmAssociatedPosition { FirmId = 1, OrderPositionId = 1, BindingType = 1, PackagePositionId = 2, ItemPositionId = 2, PrincipalPositionId = 3 },

                    // Есть две основных позиции: с отличающимся объектом привязки и с совпадающим - нет сообщения
                    new Aggregates::Firm { Id = 2 },
                    new Aggregates::Firm.FirmPosition { FirmId = 2, OrderPositionId = 3, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 2, Category1Id = 1, Category3Id = 2 },
                    new Aggregates::Firm.FirmPosition { FirmId = 2, OrderPositionId = 4, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 3, Category1Id = 1, Category3Id = 3 },
                    new Aggregates::Firm.FirmPosition { FirmId = 2, OrderPositionId = 5, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 3, Category1Id = 1, Category3Id = 2 },

                    new Aggregates::Firm.FirmAssociatedPosition { FirmId = 2, OrderPositionId = 3, BindingType = 1, PackagePositionId = 2, ItemPositionId = 2, PrincipalPositionId = 3 },

                    // Есть две основных позиции: с отличающимся объектом привязки и без учёта объекта привязки - нет сообщения (поведение отличается от erm, см Q3 для проверки)
                    new Aggregates::Firm { Id = 3 },
                    new Aggregates::Firm.FirmPosition { FirmId = 3, OrderPositionId = 6, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 2, Category1Id = 1, Category3Id = 2 },
                    new Aggregates::Firm.FirmPosition { FirmId = 3, OrderPositionId = 7, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 3, Category1Id = 1, Category3Id = 3 },
                    new Aggregates::Firm.FirmPosition { FirmId = 3, OrderPositionId = 8, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 4 },

                    new Aggregates::Firm.FirmAssociatedPosition { FirmId = 3, OrderPositionId = 6, BindingType = 1, PackagePositionId = 2, ItemPositionId = 2, PrincipalPositionId = 3 },
                    new Aggregates::Firm.FirmAssociatedPosition { FirmId = 3, OrderPositionId = 6, BindingType = 2, PackagePositionId = 2, ItemPositionId = 2, PrincipalPositionId = 4 },

                    // Есть две сопутствующих позиции с разными объектами привязки и только одна основная - ошибка есть
                    new Aggregates::Firm { Id = 4 },
                    new Aggregates::Firm.FirmPosition { FirmId = 4, OrderPositionId =  9, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 2, Category1Id = 1, Category3Id = 2 },
                    new Aggregates::Firm.FirmPosition { FirmId = 4, OrderPositionId = 10, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 2, Category1Id = 1, Category3Id = 3 },
                    new Aggregates::Firm.FirmPosition { FirmId = 4, OrderPositionId = 11, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 3, Category1Id = 1, Category3Id = 2 },

                    new Aggregates::Firm.FirmAssociatedPosition { FirmId = 4, OrderPositionId =  9, BindingType = 1, PackagePositionId = 2, ItemPositionId = 2, PrincipalPositionId = 3 },
                    new Aggregates::Firm.FirmAssociatedPosition { FirmId = 4, OrderPositionId = 10, BindingType = 1, PackagePositionId = 2, ItemPositionId = 2, PrincipalPositionId = 3 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                        new Reference<EntityTypeOrderPosition>(1,
                                            new Reference<EntityTypeOrder>(0),
                                            new Reference<EntityTypePosition>(2),
                                            new Reference<EntityTypePosition>(2)))
                                    .ToXDocument(),
                            MessageType = (int)MessageTypeCode.FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject,
                            Result = 13311,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 0,
                        },

                    new Messages::Version.ValidationResult
                    {
                        MessageParams =
                                new MessageParams(
                                        new Reference<EntityTypeOrderPosition>(10,
                                            new Reference<EntityTypeOrder>(0),
                                            new Reference<EntityTypePosition>(2),
                                            new Reference<EntityTypePosition>(2)))
                                    .ToXDocument(),
                        MessageType = (int)MessageTypeCode.FirmAssociatedPositionMustHavePrincipalWithMatchedBindingObject,
                        Result = 13311,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(2),
                        OrderId = 0,
                    });
    }
}
