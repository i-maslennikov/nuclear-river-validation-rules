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
        private static ArrangeMetadataElement OrderPositionMayMustCorrespontToActualPrice
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPositionMayMustCorrespontToActualPrice))
                .Aggregate(

                    new Aggregates::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2), IsCommitted = true },
                    new Aggregates::Order.ActualPrice { OrderId = 1, PriceId = 1 },
                    new Aggregates::Order.OrderPricePosition { OrderId = 1, OrderPositionId = 1, PriceId = ~1, IsActive = true },

                    new Aggregates::Order { Id = 2, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2), IsCommitted = false },
                    new Aggregates::Order.ActualPrice { OrderId = 2, PriceId = 2 },
                    new Aggregates::Order.OrderPricePosition { OrderId = 2, OrderPositionId = 2, PriceId = ~2, IsActive = true }
                    )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                                new Reference<EntityTypeOrderPosition>(1,
                                                    new Reference<EntityTypeOrder>(1),
                                                    new Reference<EntityTypePosition>(0))).ToXDocument(),
                            MessageType = (int)MessageTypeCode.OrderPositionMayCorrespontToActualPrice,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                                new Reference<EntityTypeOrderPosition>(2,
                                                    new Reference<EntityTypeOrder>(2),
                                                    new Reference<EntityTypePosition>(0))).ToXDocument(),
                            MessageType = (int)MessageTypeCode.OrderPositionMustCorrespontToActualPrice,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 2,
                        }
                    ).RunOnlyThis();
    }
}
