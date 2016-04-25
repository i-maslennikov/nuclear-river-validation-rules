﻿using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Domain.Model;

namespace NuClear.ValidationRules.Domain.Specifications
{
    using Aggregates = Model.Aggregates;

    public static partial class Specs
    {
        public static class Find
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

                public static FindSpecification<Aggregates::OrderPrice> OrderPrices(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<Aggregates::OrderPrice>(x => aggregateIds.Contains(x.OrderId));
                }

                public static FindSpecification<Aggregates::OrderPeriod> OrderPeriods(IReadOnlyCollection<PeriodKey> aggregateIds)
                {
                    return new FindSpecification<Aggregates::OrderPeriod>(x => aggregateIds.Any(id => id.OrganizationUnitId == x.OrganizationUnitId && id.Start == x.Start));
                }

                public static FindSpecification<Aggregates::PricePeriod> PricePeriods(IReadOnlyCollection<PeriodKey> aggregateIds)
                {
                    return new FindSpecification<Aggregates::PricePeriod>(x => aggregateIds.Any(id => id.OrganizationUnitId == x.OrganizationUnitId && id.Start == x.Start));
                }
            }
        }
    }
}