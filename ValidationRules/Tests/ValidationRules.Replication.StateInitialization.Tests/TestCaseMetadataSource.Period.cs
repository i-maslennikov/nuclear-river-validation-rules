using System;

using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    using Erm = NuClear.ValidationRules.Domain.Model.Erm;
    using Facts = NuClear.ValidationRules.Domain.Model.Facts;
    using Aggregates = NuClear.ValidationRules.Domain.Model.Aggregates;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement SingleOrderPeriod
            => ArrangeMetadataElement.Config
                                     .Name(nameof(SingleOrderPeriod))
                                     .Aggregate(
                                                new Aggregates::Order { Id = 1 },
                                                new Aggregates::Period
                                                    {
                                                        Start = DateTime.Parse("2011-01-01T00:00:00"),
                                                        End = DateTime.Parse("2011-05-01T00:00:00"),
                                                    },
                                                new Aggregates::Period
                                                    {
                                                        Start = DateTime.Parse("2011-05-01T00:00:00"),
                                                        End = DateTime.MaxValue,
                                                    },
                                                new Aggregates::OrderPeriod
                                                    {
                                                        OrderId = 1,
                                                        Start = DateTime.Parse("2011-01-01T00:00:00"),
                                                    })
                                     .Fact(
                                           new Facts::Order
                                               {
                                                   Id = 1,
                                                   BeginDistributionDate = DateTime.Parse("2011-01-01T00:00:00"),
                                                   EndDistributionDateFact = DateTime.Parse("2011-05-01T00:00:00")
                                               });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement SinglePricePeriod
            => ArrangeMetadataElement.Config
                                     .Name(nameof(SinglePricePeriod))
                                     .Aggregate(
                                                new Aggregates::Price { Id = 1 },
                                                new Aggregates::Price { Id = 2 },
                                                new Aggregates::Period
                                                    {
                                                        Start = DateTime.Parse("2011-01-01T00:00:00"),
                                                        End = DateTime.Parse("2011-02-01T00:00:00"),
                                                    },
                                                new Aggregates::Period
                                                    {
                                                        Start = DateTime.Parse("2011-02-01T00:00:00"),
                                                        End = DateTime.MaxValue,
                                                    },
                                                new Aggregates::PricePeriod { PriceId = 1, Start = DateTime.Parse("2011-01-01T00:00:00") },
                                                new Aggregates::PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-02-01T00:00:00") })
                                     .Fact(
                                           new Facts::Price
                                               {
                                                   Id = 1,
                                                   BeginDate = DateTime.Parse("2011-01-01T00:00:00"),
                                               },
                                           new Facts::Price
                                               {
                                                   Id = 2,
                                                   BeginDate = DateTime.Parse("2011-02-01T00:00:00"),
                                               });
    }
}
