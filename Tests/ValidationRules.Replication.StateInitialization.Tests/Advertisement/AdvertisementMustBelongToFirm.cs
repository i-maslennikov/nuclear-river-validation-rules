using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement AdvertisementMustBelongToFirm
            => ArrangeMetadataElement
                .Config
                .Name(nameof(AdvertisementMustBelongToFirm))
                .Fact(
                    new Facts::Order { Id = 1, FirmId = 2, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },

                    new Facts::OrderPosition { Id = 3, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 4, OrderPositionId = 3, AdvertisementId = 5, PositionId = 100 },
                    new Facts::Advertisement { Id = 5, FirmId = 2 },

                    new Facts::OrderPosition { Id = 6, OrderId = 1 },
                    new Facts::OrderPositionAdvertisement { Id = 7, OrderPositionId = 6, AdvertisementId = 8, PositionId = 101 },
                    new Facts::Advertisement { Id = 8, FirmId = 9 }
                )
                .Aggregate(
                    new Aggregates::Order { Id = 1, BeginDistributionDate = MonthStart(1), EndDistributionDatePlan = MonthStart(2) },
                    new Aggregates::Order.AdvertisementNotBelongToFirm { OrderId = 1, OrderPositionId = 6, AdvertisementId = 8, PositionId = 101, ExpectedFirmId = 2, ActualFirmId = 9 }
                )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                        new Reference<EntityTypeOrder>(1),
                                        new Reference<EntityTypeOrderPosition>(6,
                                            new Reference<EntityTypeOrder>(1),
                                            new Reference<EntityTypePosition>(101)),
                                        new Reference<EntityTypeAdvertisement>(8),
                                        new Reference<EntityTypeFirm>(2))
                                    .ToXDocument(),

                            MessageType = (int)MessageTypeCode.AdvertisementMustBelongToFirm,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        }
                );
    }
}
