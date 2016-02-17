using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Specifications;

namespace NuClear.ValidationRules.Domain.Specifications
{
    using Aggregates = Model.Aggregates;

    public static partial class Specs
    {
        public static class Find
        {
            public static class Aggs
            {
                public static FindSpecification<Aggregates::DeniedPosition> DeniedPositions(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<Aggregates::DeniedPosition>(x => aggregateIds.Cast<long?>().Contains(x.PriceId));
                }
                public static FindSpecification<Aggregates::MasterPosition> MasterPositions(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<Aggregates::MasterPosition>(x => aggregateIds.Cast<long?>().Contains(x.PriceId));
                }
                public static FindSpecification<Aggregates::AdvertisementAmountRestriction> AdvertisementAmountRestrictions(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<Aggregates::AdvertisementAmountRestriction>(x => aggregateIds.Contains(x.PriceId));
                }

                public static FindSpecification<Aggregates::OrderPosition> OrderPositions(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<Aggregates::OrderPosition>(x => aggregateIds.Contains(x.OrderId));
                }

                public static FindSpecification<Aggregates::OrderPrice> OrderPrices(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<Aggregates::OrderPrice>(x => aggregateIds.Contains(x.OrderId));
                }
            }
        }
    }
}