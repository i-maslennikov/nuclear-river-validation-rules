using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Specifications;

namespace NuClear.ValidationRules.Replication.Specifications
{
    using Aggregates = Storage.Model.PriceRules.Aggregates;

    public static partial class Specs
    {
        public static partial class Find
        {
            public static class Aggs
            {
                public static FindSpecification<Aggregates::Period> Periods(IReadOnlyCollection<PeriodKey> aggregateIds)
                {
                    var result = new FindSpecification<Aggregates::Period>(x => false);

                    return aggregateIds.Select(PeriodSpecificationForSingleKey)
                                       .Aggregate(result, (current, spec) => current | spec);
                }

                public static FindSpecification<Aggregates::OrderPeriod> OrderPeriods(IReadOnlyCollection<PeriodKey> aggregateIds)
                {
                    var result = new FindSpecification<Aggregates::OrderPeriod>(x => false);

                    return aggregateIds.Select(OrderSpecificationForSingleKey)
                                       .Aggregate(result, (current, spec) => current | spec);
                }

                public static FindSpecification<Aggregates::PricePeriod> PricePeriods(IReadOnlyCollection<PeriodKey> aggregateIds)
                {
                    var result = new FindSpecification<Aggregates::PricePeriod>(x => false);

                    return aggregateIds.Select(PriceSpecificationForSingleKey)
                                       .Aggregate(result, (current, spec) => current | spec);
                }

                private static FindSpecification<Aggregates::Period> PeriodSpecificationForSingleKey(PeriodKey periodKey)
                    => new FindSpecification<Aggregates.Period>(x => x.OrganizationUnitId == periodKey.OrganizationUnitId && periodKey.Start <= x.End && x.Start <= periodKey.End);

                private static FindSpecification<Aggregates::OrderPeriod> OrderSpecificationForSingleKey(PeriodKey periodKey)
                    => new FindSpecification<Aggregates.OrderPeriod>(x => x.OrganizationUnitId == periodKey.OrganizationUnitId && periodKey.Start <= x.Start && x.Start <= periodKey.End);

                private static FindSpecification<Aggregates::PricePeriod> PriceSpecificationForSingleKey(PeriodKey periodKey)
                    => new FindSpecification<Aggregates.PricePeriod>(x => x.OrganizationUnitId == periodKey.OrganizationUnitId && periodKey.Start <= x.Start && x.Start <= periodKey.End);
            }
        }
    }
}