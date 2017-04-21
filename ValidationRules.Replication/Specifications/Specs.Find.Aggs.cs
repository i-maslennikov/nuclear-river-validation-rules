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

                    // todo: PeriodSpecificationForSingleKey не достаточно строгая спецификация, она не использует дату окончания периода - с ней linq2db не генерирует запрос
                    return aggregateIds.Select(PeriodSpecificationForSingleKey)
                                       .Aggregate(result, (current, spec) => current | spec);
                }

                private static FindSpecification<Aggregates::Period> PeriodSpecificationForSingleKey(PeriodKey periodKey)
                    => new FindSpecification<Aggregates::Period>(x => x.OrganizationUnitId == periodKey.OrganizationUnitId && x.Start <= periodKey.End);
            }
        }
    }
}