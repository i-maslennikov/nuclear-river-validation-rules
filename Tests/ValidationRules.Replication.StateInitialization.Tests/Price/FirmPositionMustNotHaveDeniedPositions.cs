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
        private static ArrangeMetadataElement FirmPositionMustNotHaveDeniedPositions
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmPositionMustNotHaveDeniedPositions))
                .Aggregate(
                    // Сама на себя позиция не должна реагировать
                    new Aggregates::Firm { Id = 1 },
                    new Aggregates::Firm.FirmPosition { FirmId = 1, OrderPositionId = 1, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 2 },

                    new Aggregates::Firm.FirmDeniedPosition { FirmId = 1, OrderPositionId = 1, PackagePositionId = 2, ItemPositionId = 2, BindingType = 2, DeniedPositionId = 2 },

                    // Две разных позиции должны приводить к сообщению
                    new Aggregates::Firm { Id = 3 },
                    new Aggregates::Firm.FirmPosition { FirmId = 3, OrderPositionId = 4, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 2 },
                    new Aggregates::Firm.FirmPosition { FirmId = 3, OrderPositionId = 5, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 3 },

                    new Aggregates::Firm.FirmDeniedPosition { FirmId = 3, OrderPositionId = 4, PackagePositionId = 2, ItemPositionId = 2, BindingType = 2, DeniedPositionId = 3 })
                .Message(
                    new Messages::Version.ValidationResult
                    {
                        MessageParams =
                                new MessageParams(
                                        new Reference<EntityTypeOrderPosition>(4,
                                            new Reference<EntityTypeOrder>(0),
                                            new Reference<EntityTypePosition>(2),
                                            new Reference<EntityTypePosition>(2)),

                                        new Reference<EntityTypeOrderPosition>(5,
                                            new Reference<EntityTypeOrder>(0),
                                            new Reference<EntityTypePosition>(2),
                                            new Reference<EntityTypePosition>(3)))
                                    .ToXDocument(),
                        MessageType = (int)MessageTypeCode.FirmPositionMustNotHaveDeniedPositions,
                        PeriodStart = MonthStart(1),
                        PeriodEnd = MonthStart(2),
                        OrderId = 0,
                    }
                    );
    }
}
