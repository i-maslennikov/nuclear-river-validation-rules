using System;
using System.Collections.Generic;

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
        private static ArrangeMetadataElement MaximumAdvertisementAmount
            => ArrangeMetadataElement
                .Config
                .Name(nameof(MaximumAdvertisementAmount))
                .Aggregate(
                    new Aggregates::Price.AdvertisementAmountRestriction { CategoryCode = 1, CategoryName = "Category", Max = 2 },

                    new Aggregates::Order { Id = 1 },
                    new Aggregates::Order.AmountControlledPosition { OrderId = 1, CategoryCode = 1 },
                    new Aggregates::Order.OrderPeriod { OrderId = 1, Begin = MonthStart(1), End = MonthStart(3), Scope = 0 },

                    new Aggregates::Order { Id = 2 },
                    new Aggregates::Order.AmountControlledPosition { OrderId = 2, CategoryCode = 1 },
                    new Aggregates::Order.OrderPeriod { OrderId = 2, Begin = MonthStart(1), End = MonthStart(3), Scope = 0 },

                    new Aggregates::Order { Id = 3 },
                    new Aggregates::Order.AmountControlledPosition { OrderId = 3, CategoryCode = 1 },
                    new Aggregates::Order.OrderPeriod { OrderId = 3, Begin = MonthStart(1), End = MonthStart(2), Scope = -1 },

                    new Aggregates::Order { Id = 4 },
                    new Aggregates::Order.AmountControlledPosition { OrderId = 4, CategoryCode = 1 },
                    new Aggregates::Order.OrderPeriod { OrderId = 4, Begin = MonthStart(1), End = MonthStart(2), Scope = 4 },

                    new Aggregates::Order { Id = 5 },
                    new Aggregates::Order.AmountControlledPosition { OrderId = 5, CategoryCode = 1 },
                    new Aggregates::Order.OrderPeriod { OrderId = 5, Begin = MonthStart(2), End = MonthStart(3), Scope = 5 },

                    new Aggregates::Period { Start = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Period { Start = MonthStart(2), End = MonthStart(3) },
                    
                    new Aggregates::Price.PricePeriod { Begin = MonthStart(1), End = DateTime.MaxValue })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                    new Dictionary<string, object> { { "min", 0 }, { "max", 2 }, { "count", 3 }, { "name", "Category" }, { "month", MonthStart(1) } },
                                    new Reference<EntityTypeOrder>(3)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.MaximumAdvertisementAmount,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 3,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                    new Dictionary<string, object> { { "min", 0 }, { "max", 2 }, { "count", 4 }, { "name", "Category" }, { "month", MonthStart(1) } },
                                    new Reference<EntityTypeOrder>(4)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.MaximumAdvertisementAmount,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 4,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams =
                                new MessageParams(
                                    new Dictionary<string, object> { { "min", 0 }, { "max", 2 }, { "count", 3 }, { "name", "Category" }, { "month", MonthStart(2) } },
                                    new Reference<EntityTypeOrder>(5)).ToXDocument(),
                            MessageType = (int)MessageTypeCode.MaximumAdvertisementAmount,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                            OrderId = 5,
                        });
    }
}
