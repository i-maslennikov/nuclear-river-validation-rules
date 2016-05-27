using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Specifications;

namespace NuClear.ValidationRules.Replication.Specifications
{
    using Aggregates = Storage.Model.Aggregates;

    public static partial class Specs
    {
        public static partial class Find
        {
            public static class Aggs
            {
                public static FindSpecification<Aggregates::PriceDeniedPosition> PriceDeniedPositions(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<Aggregates::PriceDeniedPosition>(x => aggregateIds.Contains(x.PriceId));
                }
                public static FindSpecification<Aggregates::PriceAssociatedPosition> PriceAssociatedPositions(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<Aggregates::PriceAssociatedPosition>(x => aggregateIds.Contains(x.PriceId));
                }
                public static FindSpecification<Aggregates::AdvertisementAmountRestriction> AdvertisementAmountRestrictions(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<Aggregates::AdvertisementAmountRestriction>(x => aggregateIds.Contains(x.PriceId));
                }

                public static FindSpecification<Aggregates::RulesetRule> RulesetRules(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<Aggregates::RulesetRule>(x => aggregateIds.Contains(x.RulesetId));
                }

                public static FindSpecification<Aggregates::OrderPosition> OrderPositions(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<Aggregates::OrderPosition>(x => aggregateIds.Contains(x.OrderId));
                }

                public static FindSpecification<Aggregates::OrderPricePosition> OrderPricePositions(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<Aggregates::OrderPricePosition>(x => aggregateIds.Contains(x.OrderId));
                }

                public static FindSpecification<Aggregates::OrderPeriod> OrderPeriods(IReadOnlyCollection<PeriodKey> aggregateIds)
                {
                    var result = new FindSpecification<Aggregates::OrderPeriod>(x => false);
                    result = aggregateIds.GroupBy(x => x.OrganizationUnitId, x => x.Start)
                                         .Aggregate(result, (current, group) => current | (new FindSpecification<Aggregates.OrderPeriod>(x => x.OrganizationUnitId == group.Key) & new FindSpecification<Aggregates.OrderPeriod>(x => group.Contains(x.Start))));
                    return result;
                }

                public static FindSpecification<Aggregates::PricePeriod> PricePeriods(IReadOnlyCollection<PeriodKey> aggregateIds)
                {
                    var result = new FindSpecification<Aggregates::PricePeriod>(x => false);
                    result = aggregateIds.GroupBy(x => x.OrganizationUnitId, x => x.Start)
                                         .Aggregate(result, (current, group) => current | (new FindSpecification<Aggregates.PricePeriod>(x => x.OrganizationUnitId == group.Key) & new FindSpecification<Aggregates.PricePeriod>(x => group.Contains(x.Start))));
                    return result;
                }
            }
        }
    }
}