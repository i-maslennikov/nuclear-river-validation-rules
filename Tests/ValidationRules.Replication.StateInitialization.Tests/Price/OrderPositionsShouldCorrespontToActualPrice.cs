using System;
using System.Xml.Linq;

using NuClear.DataTest.Metamodel.Dsl;


using Erm = NuClear.ValidationRules.Storage.Model.Erm;
using Aggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using Messages = NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPositionsShouldCorrespontToActualPrice
            => ArrangeMetadataElement.Config
                    .Name(nameof(PriceContextSupport))
                    .Aggregate(
                        new Aggregates::Order { Id = 1, Number = "InvalidOrder" },
                        new Aggregates::OrderPeriod { OrderId = 1, Start = FirstDayJan },
                        new Aggregates::OrderPeriod { OrderId = 1, Start = FirstDayFeb },

                        new Aggregates::Order { Id = 2, Number = "ValidOrder" },
                        new Aggregates::OrderPeriod { OrderId = 2, Start = FirstDayFeb },

                        new Aggregates::Period { Start = FirstDayJan, End = FirstDayFeb },
                        new Aggregates::Period { Start = FirstDayFeb, End = FirstDayMar },

                        new Aggregates::PricePeriod { Start = FirstDayFeb },

                        new Aggregates::Price())
                    .Message(
                        new Messages::Version.ValidationResult
                            {
                                MessageParams = XDocument.Parse("<root><order id = \"1\" number=\"InvalidOrder\" /></root>"),
                                MessageType = (int)MessageTypeCode.OrderPositionsShouldCorrespontToActualPrice,
                                Result = 255,
                                PeriodStart = FirstDayJan,
                                PeriodEnd = FirstDayMar,
                            });
    }
}
