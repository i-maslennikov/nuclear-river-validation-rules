using System;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;

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
                    new Aggregates::AdvertisementAmountRestriction { CategoryCode = 1, CategoryName = "Category", Max = 2 },

                    new Aggregates::Order { Id = 1, Number = "Order" },
                    new Aggregates::AmountControlledPosition { OrderId = 1, CategoryCode = 1 },
                    new Aggregates::OrderPeriod { OrderId = 1, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::OrderPeriod { OrderId = 1, Start = MonthStart(2), Scope = 0 },

                    new Aggregates::Order { Id = 2, Number = "Order" },
                    new Aggregates::AmountControlledPosition { OrderId = 2, CategoryCode = 1 },
                    new Aggregates::OrderPeriod { OrderId = 2, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::OrderPeriod { OrderId = 2, Start = MonthStart(2), Scope = 0 },

                    new Aggregates::Order { Id = 3, Number = "Order" },
                    new Aggregates::AmountControlledPosition { OrderId = 3, CategoryCode = 1 },
                    new Aggregates::OrderPeriod { OrderId = 3, Start = MonthStart(1), Scope = -1 },

                    new Aggregates::Order { Id = 4, Number = "Order" },
                    new Aggregates::AmountControlledPosition { OrderId = 4, CategoryCode = 1 },
                    new Aggregates::OrderPeriod { OrderId = 4, Start = MonthStart(1), Scope = 4 },

                    new Aggregates::Order { Id = 5, Number = "Order" },
                    new Aggregates::AmountControlledPosition { OrderId = 5, CategoryCode = 1 },
                    new Aggregates::OrderPeriod { OrderId = 5, Start = MonthStart(2), Scope = 5 },

                    new Aggregates::Period { Start = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Period { Start = MonthStart(2), End = MonthStart(3) },
                    new Aggregates::PricePeriod { Start = MonthStart(1) },
                    new Aggregates::PricePeriod { Start = MonthStart(2) })
                .Message(
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<message min=\"0\" max=\"2\" count=\"3\" name=\"Category\" month=\"2012-01-01T00:00:00\" />" +
                                                            "<order id=\"3\" number=\"Order\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.MaximumAdvertisementAmount,
                            Result = 243,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 3,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<message min=\"0\" max=\"2\" count=\"4\" name=\"Category\" month=\"2012-01-01T00:00:00\" />" +
                                                            "<order id=\"4\" number=\"Order\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.MaximumAdvertisementAmount,
                            Result = 243,
                            PeriodStart = MonthStart(1),
                            PeriodEnd = MonthStart(2),
                            OrderId = 4,
                        },
                    new Messages::Version.ValidationResult
                        {
                            MessageParams = XDocument.Parse("<root>" +
                                                            "<message min=\"0\" max=\"2\" count=\"3\" name=\"Category\" month=\"2012-02-01T00:00:00\" />" +
                                                            "<order id=\"5\" number=\"Order\" />" +
                                                            "</root>"),
                            MessageType = (int)MessageTypeCode.MaximumAdvertisementAmount,
                            Result = 243,
                            PeriodStart = MonthStart(2),
                            PeriodEnd = MonthStart(3),
                            OrderId = 5,
                        });
    }
}
