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
        private static ArrangeMetadataElement OrderPositionsShouldCorrespontToActualPrice
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPositionsShouldCorrespontToActualPrice))
                .Aggregate(

                    new Aggregates::Order { Id = 1 },
                    new Aggregates::Order.ActualPrice { OrderId = 1, PriceId = 1 },

                    new Aggregates::Order { Id = 2 },
                    new Aggregates::Order.ActualPrice { OrderId = 2, PriceId = null },

                    new Aggregates::Period { OrganizationUnitId = 1, Start = FirstDayJan, End = FirstDayFeb },
                    new Aggregates::Period.OrderPeriod { OrganizationUnitId = 1, Start = FirstDayJan, OrderId = 1 },
                    new Aggregates::Period.OrderPeriod { OrganizationUnitId = 1, Start = FirstDayJan, OrderId = 2 }
                    )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(new Reference<EntityTypeOrder>(2)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.OrderPositionsShouldCorrespontToActualPrice,
                            PeriodStart = FirstDayJan,
                            PeriodEnd = FirstDayFeb,
                            OrderId = 2,
                        });
    }
}
