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
            => ArrangeMetadataElement
                .Config
                .Name(nameof(OrderPeriod))
                .Fact(
                    new Facts::Order { Id = 1, BeginDistributionDate = MonthStart(1), EndDistributionDateFact = MonthStart(2), EndDistributionDatePlan = MonthStart(2), WorkflowStepId = 1 },
                    new Facts::Order { Id = 2, BeginDistributionDate = MonthStart(1), EndDistributionDateFact = MonthStart(2), EndDistributionDatePlan = MonthStart(2), WorkflowStepId = 2 },
                    new Facts::Order { Id = 3, BeginDistributionDate = MonthStart(1), EndDistributionDateFact = MonthStart(2), EndDistributionDatePlan = MonthStart(2), WorkflowStepId = 5 },
                    new Facts::Order { Id = 4, BeginDistributionDate = MonthStart(1), EndDistributionDateFact = MonthStart(2), EndDistributionDatePlan = MonthStart(3), WorkflowStepId = 4 })
                .Aggregate(
                    new Aggregates::OrderPeriod { OrderId = 1, Start = MonthStart(1), Scope = 1 },
                    new Aggregates::OrderPeriod { OrderId = 2, Start = MonthStart(1), Scope = -1 },
                    new Aggregates::OrderPeriod { OrderId = 3, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::OrderPeriod { OrderId = 4, Start = MonthStart(1), Scope = 0 },
                    new Aggregates::OrderPeriod { OrderId = 4, Start = MonthStart(2), Scope = 4 });

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
                    new Aggregates::PricePeriod { PriceId = 1, Start = DateTime.Parse("2011-01-01") },
                    new Aggregates::PricePeriod { PriceId = 2, Start = DateTime.Parse("2011-02-01") })
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

                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistributionDate = MonthStart(2), EndDistributionDateFact = MonthStart(3), EndDistributionDatePlan = MonthStart(4) },
                    new Facts::Order { Id = 3, DestOrganizationUnitId = 1, BeginDistributionDate = MonthStart(5), EndDistributionDateFact = MonthStart(7), EndDistributionDatePlan = MonthStart(7) })
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
