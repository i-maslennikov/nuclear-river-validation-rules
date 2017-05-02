using NuClear.DataTest.Metamodel.Dsl;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Aggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement ActualPrice
            => ArrangeMetadataElement
                .Config
                .Name(nameof(ActualPrice))
                .Fact(
                    new Facts::Order { Id = 1, DestOrganizationUnitId = 1, BeginDistribution = MonthStart(2) },
                    new Facts::Price { Id = 1, ProjectId = 123, BeginDate = MonthStart(1) },
                    new Facts::Price { Id = 2, ProjectId = 123, BeginDate = MonthStart(2) },
                    new Facts::Price { Id = 3, ProjectId = 123, BeginDate = MonthStart(3) },
                    new Facts::Project { Id = 123, OrganizationUnitId = 1 })
                .Aggregate(
                    new Aggregates::Order.ActualPrice { OrderId = 1, PriceId = 2 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderMustHaveActualPrice
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderMustHaveActualPrice))
                .Aggregate(

                    new Aggregates::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.ActualPrice { OrderId = 1, PriceId = 1 },

                    new Aggregates::Order { Id = 2, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.ActualPrice { OrderId = 2, PriceId = null }
                    )
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(new Reference<EntityTypeOrder>(2)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.OrderMustHaveActualPrice,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 2,
                        });
    }
}
