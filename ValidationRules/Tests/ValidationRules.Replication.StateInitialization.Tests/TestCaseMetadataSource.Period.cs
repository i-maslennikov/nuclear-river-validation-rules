using System;

using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    using Facts = NuClear.ValidationRules.Domain.Model.Facts;
    using Aggregates = NuClear.ValidationRules.Domain.Model.Aggregates;

    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPeriod
            => ArrangeMetadataElement.Config
                                     .Name(nameof(OrderPeriod))
                                     .Aggregate(
                                                new Aggregates::Order { Id = 1 },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-01-01"), End = DateTime.Parse("2011-05-01") },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-05-01"), End = DateTime.MaxValue },
                                                new Aggregates::OrderPeriod { OrderId = 1, Start = DateTime.Parse("2011-01-01") })
                                     .Fact(
                                           new Facts::Order { Id = 1, BeginDistributionDate = DateTime.Parse("2011-01-01"), EndDistributionDateFact = DateTime.Parse("2011-05-01") });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PricePeriod
            => ArrangeMetadataElement.Config
                                     .Name(nameof(PricePeriod))
                                     .Aggregate(
                                                new Aggregates::Price { Id = 1 },
                                                new Aggregates::Price { Id = 2 },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-01-01"), End = DateTime.Parse("2011-02-01") },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-02-01"), End = DateTime.MaxValue },
                                                new Aggregates::PricePeriod { PriceId = 1, Start = DateTime.Parse("2011-01-01") },
                                                new Aggregates::PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-02-01") })
                                     .Fact(
                                           new Facts::Price { Id = 1, BeginDate = DateTime.Parse("2011-01-01") },
                                           new Facts::Price { Id = 2, BeginDate = DateTime.Parse("2011-02-01") });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PriceAndOrderPeriod
            => ArrangeMetadataElement.Config
                                     .Name(nameof(PriceAndOrderPeriod))
                                     .Aggregate(
                                                new Aggregates::Price { Id = 1 },
                                                new Aggregates::Price { Id = 2 },
                                                new Aggregates::Price { Id = 3 },
                                                new Aggregates::Order { Id = 1 },
                                                new Aggregates::Order { Id = 2 },
                                                new Aggregates::Order { Id = 3 },

                                                new Aggregates::Period { Start = DateTime.Parse("2011-01-01"), End = DateTime.Parse("2011-02-01") },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-02-01"), End = DateTime.Parse("2011-03-01") },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-03-01"), End = DateTime.Parse("2011-04-01") },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-04-01"), End = DateTime.Parse("2011-05-01") },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-05-01"), End = DateTime.Parse("2011-06-01") },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-06-01"), End = DateTime.MaxValue },

                                                // Для второго отделения организации должны быть свои периоды, не влияющие на первое
                                                new Aggregates::Period { Start = DateTime.Parse("2010-01-01"), End = DateTime.Parse("2010-02-01"), OrganizationUnitId = 1},
                                                new Aggregates::Period { Start = DateTime.Parse("2010-02-01"), End = DateTime.MaxValue, OrganizationUnitId = 1 },

                                                new Aggregates::PricePeriod { PriceId = 1, Start = DateTime.Parse("2011-01-01") },
                                                new Aggregates::PricePeriod { PriceId = 1, Start = DateTime.Parse("2011-02-01") },

                                                new Aggregates::PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-03-01") },
                                                new Aggregates::PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-04-01") },
                                                new Aggregates::PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-05-01") },
                                                new Aggregates::PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-06-01") },

                                                new Aggregates::PricePeriod { PriceId = 3, Start = DateTime.Parse("2010-01-01"), OrganizationUnitId = 1 },
                                                new Aggregates::PricePeriod { PriceId = 3, Start = DateTime.Parse("2010-02-01"), OrganizationUnitId = 1 },

                                                new Aggregates::OrderPeriod { OrderId = 1, Start = DateTime.Parse("2011-02-01") },
                                                new Aggregates::OrderPeriod { OrderId = 1, Start = DateTime.Parse("2011-03-01") },

                                                new Aggregates::OrderPeriod { OrderId = 2, Start = DateTime.Parse("2011-05-01") },

                                                new Aggregates::OrderPeriod { OrderId = 3, Start = DateTime.Parse("2010-01-01"), OrganizationUnitId = 1 }
                                                )
                                     .Fact(
                                           new Facts::Price { Id = 1, BeginDate = DateTime.Parse("2011-01-01") },
                                           new Facts::Price { Id = 2, BeginDate = DateTime.Parse("2011-03-01") },
                                           new Facts::Price { Id = 3, OrganizationUnitId = 1, BeginDate = DateTime.Parse("2010-01-01") },

                                           new Facts::Order { Id = 1, BeginDistributionDate = DateTime.Parse("2011-02-01"), EndDistributionDateFact = DateTime.Parse("2011-04-01") },
                                           new Facts::Order { Id = 2, BeginDistributionDate = DateTime.Parse("2011-05-01"), EndDistributionDateFact = DateTime.Parse("2011-06-01") },
                                           new Facts::Order { Id = 3, DestOrganizationUnitId = 1, BeginDistributionDate = DateTime.Parse("2010-01-01"), EndDistributionDateFact = DateTime.Parse("2010-02-01") }
                                           );
    }
}
