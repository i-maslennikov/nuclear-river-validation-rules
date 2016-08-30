using System;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPeriod
            => ArrangeMetadataElement.Config
                                     .Name(nameof(OrderPeriod))
                                     .Aggregate(
                                                new Aggregates::Order { Id = 1 },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-01-01"), End = DateTime.Parse("2011-05-01"), ProjectId = 123 },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-05-01"), End = DateTime.Parse("2011-06-01"), ProjectId = 123 },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-06-01"), End = DateTime.MaxValue, ProjectId = 123 },
                                                new Aggregates::OrderPeriod { OrderId = 1, Start = DateTime.Parse("2011-01-01") },
                                                new Aggregates::OrderPeriod { OrderId = 1, Start = DateTime.Parse("2011-05-01") })
                                     .Fact(
                                           new Facts::Order { Id = 1, BeginDistributionDate = DateTime.Parse("2011-01-01"), EndDistributionDateFact = DateTime.Parse("2011-05-01"), EndDistributionDatePlan = DateTime.Parse("2011-06-01")},
                                           new Facts::Project {Id = 123 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PricePeriod
            => ArrangeMetadataElement.Config
                                     .Name(nameof(PricePeriod))
                                     .Aggregate(
                                                new Aggregates::Price { Id = 1 },
                                                new Aggregates::Price { Id = 2 },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-01-01"), End = DateTime.Parse("2011-02-01"), ProjectId = 123 },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-02-01"), End = DateTime.MaxValue, ProjectId = 123 },
                                                new Aggregates::PricePeriod { PriceId = 1, Start = DateTime.Parse("2011-01-01") },
                                                new Aggregates::PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-02-01") })
                                     .Fact(
                                           new Facts::Price { Id = 1, BeginDate = DateTime.Parse("2011-01-01") },
                                           new Facts::Price { Id = 2, BeginDate = DateTime.Parse("2011-02-01") },
                                           new Facts::Project { Id = 123 });

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

                                                new Aggregates::Period { Start = DateTime.Parse("2011-01-01"), End = DateTime.Parse("2011-02-01"), OrganizationUnitId = 2 },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-02-01"), End = DateTime.Parse("2011-03-01"), OrganizationUnitId = 2 },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-03-01"), End = DateTime.Parse("2011-04-01"), OrganizationUnitId = 2 },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-04-01"), End = DateTime.Parse("2011-05-01"), OrganizationUnitId = 2 },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-05-01"), End = DateTime.Parse("2011-06-01"), OrganizationUnitId = 2 },
                                                new Aggregates::Period { Start = DateTime.Parse("2011-06-01"), End = DateTime.MaxValue, OrganizationUnitId = 2 },

                                                // Для второго отделения организации должны быть свои периоды, не влияющие на первое
                                                new Aggregates::Period { Start = DateTime.Parse("2010-01-01"), End = DateTime.Parse("2010-02-01"), OrganizationUnitId = 1},
                                                new Aggregates::Period { Start = DateTime.Parse("2010-02-01"), End = DateTime.Parse("2010-03-01"), OrganizationUnitId = 1},
                                                new Aggregates::Period { Start = DateTime.Parse("2010-03-01"), End = DateTime.MaxValue, OrganizationUnitId = 1 },

                                                new Aggregates::PricePeriod { PriceId = 1, Start = DateTime.Parse("2011-01-01"), OrganizationUnitId = 2 },
                                                new Aggregates::PricePeriod { PriceId = 1, Start = DateTime.Parse("2011-02-01"), OrganizationUnitId = 2 },

                                                new Aggregates::PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-03-01"), OrganizationUnitId = 2 },
                                                new Aggregates::PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-04-01"), OrganizationUnitId = 2 },
                                                new Aggregates::PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-05-01"), OrganizationUnitId = 2 },
                                                new Aggregates::PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-06-01"), OrganizationUnitId = 2 },

                                                new Aggregates::PricePeriod { PriceId = 3, Start = DateTime.Parse("2010-01-01"), OrganizationUnitId = 1 },
                                                new Aggregates::PricePeriod { PriceId = 3, Start = DateTime.Parse("2010-02-01"), OrganizationUnitId = 1 },
                                                new Aggregates::PricePeriod { PriceId = 3, Start = DateTime.Parse("2010-03-01"), OrganizationUnitId = 1 },

                                                new Aggregates::OrderPeriod { OrderId = 1, Start = DateTime.Parse("2011-02-01"), OrganizationUnitId = 2 },
                                                new Aggregates::OrderPeriod { OrderId = 1, Start = DateTime.Parse("2011-03-01"), OrganizationUnitId = 2 },

                                                new Aggregates::OrderPeriod { OrderId = 2, Start = DateTime.Parse("2011-05-01"), OrganizationUnitId = 2 },

                                                new Aggregates::OrderPeriod { OrderId = 3, Start = DateTime.Parse("2010-01-01"), OrganizationUnitId = 1 },
                                                new Aggregates::OrderPeriod { OrderId = 3, Start = DateTime.Parse("2010-02-01"), OrganizationUnitId = 1 }
                                                )
                                     .Fact(
                                           new Facts::Price { Id = 1, OrganizationUnitId = 2, BeginDate = DateTime.Parse("2011-01-01") },
                                           new Facts::Price { Id = 2, OrganizationUnitId = 2, BeginDate = DateTime.Parse("2011-03-01") },
                                           new Facts::Price { Id = 3, OrganizationUnitId = 1, BeginDate = DateTime.Parse("2010-01-01") },

                                           new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistributionDate = DateTime.Parse("2011-02-01"), EndDistributionDateFact = DateTime.Parse("2011-04-01"), EndDistributionDatePlan = DateTime.Parse("2011-04-01") },
                                           new Facts::Order { Id = 2, DestOrganizationUnitId = 2, BeginDistributionDate = DateTime.Parse("2011-05-01"), EndDistributionDateFact = DateTime.Parse("2011-06-01"), EndDistributionDatePlan = DateTime.Parse("2011-06-01") },
                                           new Facts::Order { Id = 3, DestOrganizationUnitId = 1, BeginDistributionDate = DateTime.Parse("2010-01-01"), EndDistributionDateFact = DateTime.Parse("2010-02-01"), EndDistributionDatePlan = DateTime.Parse("2010-03-01") });
    }
}
