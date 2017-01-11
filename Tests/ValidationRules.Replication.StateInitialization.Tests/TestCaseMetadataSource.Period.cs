using System;

using NuClear.DataTest.Metamodel.Dsl;

using Aggregates = NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;
using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public sealed partial class TestCaseMetadataSource
    {
        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement OrderPeriod
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPeriod))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(2), EndDistributionPlan = MonthStart(2), WorkflowStep = 1 },
                    new Facts::Order { Id = 2, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(2), EndDistributionPlan = MonthStart(2), WorkflowStep = 2 },
                    new Facts::Order { Id = 3, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(2), EndDistributionPlan = MonthStart(2), WorkflowStep = 5 },
                    new Facts::Order { Id = 4, BeginDistribution = MonthStart(1), EndDistributionFact = MonthStart(2), EndDistributionPlan = MonthStart(3), WorkflowStep = 4 })
                .Aggregate(
                    new Aggregates::Period.OrderPeriod { OrderId = 1, Start = MonthStart(1), Scope = 1 },
                    new Aggregates::Period.OrderPeriod { OrderId = 2, Start = MonthStart(1), Scope = -1 },
                    new Aggregates::Period.OrderPeriod { OrderId = 3, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::Period.OrderPeriod { OrderId = 4, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::Period.OrderPeriod { OrderId = 4, Start = MonthStart(2), Scope = 4 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PricePeriod
            => ArrangeMetadataElement
                .Config
                .Name(nameof(PricePeriod))
                .Aggregate(
                    new Aggregates::Project { Id = 123 },
                    new Aggregates::Price { Id = 1, BeginDate = DateTime.Parse("2011-01-01") },
                    new Aggregates::Price { Id = 2, BeginDate = DateTime.Parse("2011-02-01") },
                    new Aggregates::Period { Start = DateTime.Parse("2011-01-01"), End = DateTime.Parse("2011-02-01"), ProjectId = 123 },
                    new Aggregates::Period { Start = DateTime.Parse("2011-02-01"), End = DateTime.MaxValue, ProjectId = 123 },
                    new Aggregates::Period.PricePeriod { PriceId = 1, Start = DateTime.Parse("2011-01-01") },
                    new Aggregates::Period.PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-02-01") })
                .Fact(
                    new Facts::Price { Id = 1, BeginDate = DateTime.Parse("2011-01-01") },
                    new Facts::Price { Id = 2, BeginDate = DateTime.Parse("2011-02-01") },
                    new Facts::Project { Id = 123 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement Period
            => ArrangeMetadataElement
                .Config
                .Name(nameof(Period))
                .Fact(
                    new Facts::Price { Id = 1, OrganizationUnitId = 2, BeginDate = MonthStart(1) },
                    new Facts::Price { Id = 2, OrganizationUnitId = 2, BeginDate = MonthStart(5) },
                    new Facts::Price { Id = 3, OrganizationUnitId = 1, BeginDate = MonthStart(3) },

                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = MonthStart(2), EndDistributionFact = MonthStart(3), EndDistributionPlan = MonthStart(4) },
                    new Facts::Order { Id = 3, DestOrganizationUnitId = 1, BeginDistribution = MonthStart(5), EndDistributionFact = MonthStart(7), EndDistributionPlan = MonthStart(7) })
                .Aggregate(
                    new Aggregates::Period { Start = MonthStart(1), End = MonthStart(2), OrganizationUnitId = 2 },
                    new Aggregates::Period { Start = MonthStart(2), End = MonthStart(3), OrganizationUnitId = 2 },
                    new Aggregates::Period { Start = MonthStart(3), End = MonthStart(4), OrganizationUnitId = 2 },
                    new Aggregates::Period { Start = MonthStart(4), End = MonthStart(5), OrganizationUnitId = 2 },
                    new Aggregates::Period { Start = MonthStart(5), End = DateTime.MaxValue, OrganizationUnitId = 2 },

                    // Для второго отделения организации должны быть свои периоды, не влияющие на первое
                    new Aggregates::Period { Start = MonthStart(3), End = MonthStart(5), OrganizationUnitId = 1 },
                    new Aggregates::Period { Start = MonthStart(5), End = MonthStart(7), OrganizationUnitId = 1 },
                    new Aggregates::Period { Start = MonthStart(7), End = DateTime.MaxValue, OrganizationUnitId = 1 });
    }
}
