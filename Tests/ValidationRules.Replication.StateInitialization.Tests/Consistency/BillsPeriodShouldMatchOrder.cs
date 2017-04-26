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
        private static ArrangeMetadataElement BillsPeriodShouldMatchOrder
            => ArrangeMetadataElement
                .Config
                .Name(nameof(BillsPeriodShouldMatchOrder))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Facts::Bill { Id = 1, OrderId = 1, Begin = MonthStart(1), End = MonthStart(2) },
                    new Facts::Bill { Id = 2, OrderId = 1, Begin = MonthStart(2), End = MonthStart(2).AddDays(1) })
                .Aggregate(
                    new Aggregates::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionPlan = MonthStart(2) },
                    new Aggregates::Order.InvalidBillsPeriod { OrderId = 1 })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = new MessageParams(
                                    new Reference<EntityTypeOrder>(1))
                                .ToXDocument(),
                            MessageType = (int)MessageTypeCode.BillsPeriodShouldMatchOrder,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 1,
                        });
    }
}
