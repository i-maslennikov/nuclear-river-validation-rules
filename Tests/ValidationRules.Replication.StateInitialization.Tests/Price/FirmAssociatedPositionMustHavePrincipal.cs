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
        private static ArrangeMetadataElement FirmAssociatedPositionMustHavePrincipal
            => ArrangeMetadataElement
                .Config
                .Name(nameof(FirmAssociatedPositionMustHavePrincipal))
                .Aggregate(
                    new Aggregates::Firm { Id = 1 },

                    new Aggregates::Firm.FirmPosition { FirmId = 1, OrderPositionId = 1, Begin = MonthStart(1), End = MonthStart(2), PackagePositionId = 2, ItemPositionId = 2 },
                    new Aggregates::Firm.FirmPosition { FirmId = 1, OrderPositionId = 1, Begin = MonthStart(2), End = MonthStart(3), PackagePositionId = 2, ItemPositionId = 2 },
                    new Aggregates::Firm.FirmPosition { FirmId = 1, OrderPositionId = 4, Begin = MonthStart(2), End = MonthStart(3), PackagePositionId = 2, ItemPositionId = 3 },

                    new Aggregates::Firm.FirmAssociatedPosition { FirmId = 1, OrderPositionId = 1, BindingType = 2, PackagePositionId = 2, ItemPositionId = 2, PrincipalPositionId = 3 })
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
                            MessageType = (int)MessageTypeCode.FirmAssociatedPositionMustHavePrincipal,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 0,
                        });
    }
}
