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
                    new Aggregates::Order.OrderPeriod { OrderId = 1, Begin = MonthStart(1), End = MonthStart(2), Scope = 1 },
                    new Aggregates::Order.OrderPeriod { OrderId = 2, Begin = MonthStart(1), End = MonthStart(2), Scope = -1 },
                    new Aggregates::Order.OrderPeriod { OrderId = 3, Begin = MonthStart(1), End = MonthStart(2), Scope = 0 },
                    new Aggregates::Order.OrderPeriod { OrderId = 4, Begin = MonthStart(1), End = MonthStart(2), Scope = 0 },
                    new Aggregates::Order.OrderPeriod { OrderId = 4, Begin = MonthStart(2), End = MonthStart(3), Scope = 4 });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement PricePeriod
            => ArrangeMetadataElement
                .Config
                .Name(nameof(PricePeriod))
                .Fact(
                    new Facts::Price { Id = 1, BeginDate = MonthStart(1) },
                    new Facts::Price { Id = 2, BeginDate = MonthStart(2) },
                    new Facts::Project { Id = 123 })
                .Aggregate(
                    new Aggregates::Price { Id = 1 },
                    new Aggregates::Price { Id = 2 },
                    new Aggregates::Price.PricePeriod { PriceId = 1, ProjectId = 123, Begin = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Price.PricePeriod { PriceId = 2, ProjectId = 123, Begin = MonthStart(2), End = DateTime.MaxValue });

        // ReSharper disable once UnusedMember.Local
        private static ArrangeMetadataElement Period
            => ArrangeMetadataElement
                .Config
                .Name(nameof(Period))
                .Fact(
                    new Facts::Price { Id = 1, OrganizationUnitId = 2, BeginDate = MonthStart(1) },
                    new Facts::Price { Id = 2, OrganizationUnitId = 2, BeginDate = MonthStart(5) },
                    new Facts::Price { Id = 3, OrganizationUnitId = 1, BeginDate = MonthStart(3) },

                    new Facts::Project { Id = 1, OrganizationUnitId = 1 },
                    new Facts::Project { Id = 2, OrganizationUnitId = 2 },

                    new Facts::Order { Id = 1, DestOrganizationUnitId = 2, BeginDistribution = MonthStart(2), EndDistributionFact = MonthStart(3), EndDistributionPlan = MonthStart(4) },
                    new Facts::Order { Id = 3, DestOrganizationUnitId = 1, BeginDistribution = MonthStart(5), EndDistributionFact = MonthStart(7), EndDistributionPlan = MonthStart(7) })
                .Aggregate(
                    new Aggregates::Period { Start = MonthStart(1), End = MonthStart(2) },
                    new Aggregates::Period { Start = MonthStart(2), End = MonthStart(3) },
                    new Aggregates::Period { Start = MonthStart(3), End = MonthStart(4) },
                    new Aggregates::Period { Start = MonthStart(4), End = MonthStart(5) },
                    new Aggregates::Period { Start = MonthStart(5), End = MonthStart(7) },
                    new Aggregates::Period { Start = MonthStart(7), End = DateTime.MaxValue });
    }
}
