using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Domain.Model.Aggregates;

namespace NuClear.ValidationRules.Domain.Specifications
{
    public static partial class Specs
    {
        public static class Find
        {
            public static class Aggs
            {
                public static FindSpecification<DeniedPosition> DeniedPositions(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<DeniedPosition>(x => aggregateIds.Cast<long?>().Contains(x.PriceId));
                }
                public static FindSpecification<MasterPosition> MasterPositions(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<MasterPosition>(x => aggregateIds.Cast<long?>().Contains(x.PriceId));
                }
                public static FindSpecification<AdvertisementAmountRestriction> AdvertisementAmountRestrictions(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<AdvertisementAmountRestriction>(x => aggregateIds.Contains(x.PriceId));
                }

                public static FindSpecification<OrderPosition> OrderPositions(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<OrderPosition>(x => aggregateIds.Contains(x.OrderId));
                }

                public static FindSpecification<OrderPrice> OrderPrices(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<OrderPrice>(x => aggregateIds.Contains(x.OrderId));
                }
            }
        }
    }
}