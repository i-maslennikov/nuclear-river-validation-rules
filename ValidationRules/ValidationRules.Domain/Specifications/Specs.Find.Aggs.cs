using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Domain.Model.Aggregates;

namespace NuClear.ValidationRules.Domain.Specifications
{
    public static partial class Specs
    {
        public static partial class Find
        {
            public static partial class Aggs
            {
                public static FindSpecification<DeniedPosition> DeniedPositions(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<DeniedPosition>(x => aggregateIds.Cast<long?>().Contains(x.PriceId));
                }
            }
        }
    }
}