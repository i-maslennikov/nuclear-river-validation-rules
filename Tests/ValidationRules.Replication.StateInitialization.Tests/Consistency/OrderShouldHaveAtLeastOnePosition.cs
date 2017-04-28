using System;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

using Aggregates = NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;
using MessageTypeCode = NuClear.ValidationRules.Storage.Model.Messages.MessageTypeCode;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderShouldHaveAtLeastOnePosition
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderShouldHaveAtLeastOnePosition))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) })
                .Aggregate(
                    new Aggregates::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.HasNoAnyPosition { OrderId = 1 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Reference<EntityTypeOrder>(1))
                                .ToXDocument(),
                            MessageType = (int)MessageTypeCode.OrderShouldHaveAtLeastOnePosition,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        });
    }
}
