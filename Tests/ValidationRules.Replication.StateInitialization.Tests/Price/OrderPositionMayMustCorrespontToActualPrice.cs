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
        private static ArrangeMetadataElement OrderPositionMayMustCorrespontToActualPrice
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPositionMayMustCorrespontToActualPrice))
                .Aggregate(

                    new Aggregates::Order { Id = 1 },
                    new Aggregates::Order.ActualPrice { OrderId = 1, PriceId = 1 },
                    new Aggregates::Order.OrderPricePosition { OrderId = 1, OrderPositionId = 1, PriceId = ~1, IsActive = true },

                    new Aggregates::Order { Id = 2 },
                    new Aggregates::Order.ActualPrice { OrderId = 2, PriceId = 2 },
                    new Aggregates::Order.OrderPricePosition { OrderId = 2, OrderPositionId = 2, PriceId = ~2, IsActive = true },

                    new Aggregates::Period { OrganizationUnitId = 1, Start = FirstDayJan, End = FirstDayFeb },
                    new Aggregates::Period.OrderPeriod { OrganizationUnitId = 1, Start = FirstDayJan, OrderId = 1, Scope = 0 },
                    new Aggregates::Period.OrderPeriod { OrganizationUnitId = 1, Start = FirstDayJan, OrderId = 2, Scope = ~0 }
                    )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                                new Reference<EntityTypeOrderPosition>(1,
                                                    new Reference<EntityTypeOrder>(1),
                                                    new Reference<EntityTypePosition>(0))).ToXDocument(),
                            MessageType = (int)MessageTypeCode.OrderPositionMayCorrespontToActualPrice,
                            PeriodStart = FirstDayJan,
                            PeriodEnd = FirstDayFeb,
                            OrderId = 1,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                                new Reference<EntityTypeOrderPosition>(2,
                                                    new Reference<EntityTypeOrder>(2),
                                                    new Reference<EntityTypePosition>(0))).ToXDocument(),
                            MessageType = (int)MessageTypeCode.OrderPositionMustCorrespontToActualPrice,
                            PeriodStart = FirstDayJan,
                            PeriodEnd = FirstDayFeb,
                            OrderId = 2,
                        }
                    ).RunOnlyThis();
    }
}
