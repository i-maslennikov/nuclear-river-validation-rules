using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.PriceRules.Validation.Dto;

namespace NuClear.ValidationRules.Replication.Specifications
{
    using Aggregates = Storage.Model.PriceRules.Aggregates;

    public static partial class Specs
    {
        public static partial class Find
        {
            public static class Aggs
            {
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

                /// <summary>
                /// Выражение сравнения объектов привязки.
                /// Не пытайтесь понять смысл этого выражения - это прекрасный образец write-only.
                /// В настоящий момент оно реализует таблицу, представленную в документации:
                /// https://github.com/2gis/nuclear-river/blob/feature/validation-rules/docs/ru/validation-rules/compare-linking-objects.md
                /// </summary>
                public static Expression<Func<PrincipalAssociatedPostionPair, bool>> BindingObjectMatch()
                {
                    return x =>
                        (x.OrderAssociatedPosition.HasNoBinding == x.OrderPrincipalPosition.HasNoBinding) &&
                         ((x.OrderAssociatedPosition.Category1Id == x.OrderPrincipalPosition.Category1Id) &&
                          (x.OrderAssociatedPosition.Category3Id == x.OrderPrincipalPosition.Category3Id || x.OrderAssociatedPosition.Category3Id == null || x.OrderPrincipalPosition.Category3Id == null) &&
                          (x.OrderAssociatedPosition.FirmAddressId == x.OrderPrincipalPosition.FirmAddressId || x.OrderAssociatedPosition.FirmAddressId == null || x.OrderPrincipalPosition.FirmAddressId == null) ||
                          (x.OrderAssociatedPosition.Category1Id == x.OrderPrincipalPosition.Category1Id || x.OrderAssociatedPosition.Category1Id == null || x.OrderPrincipalPosition.Category1Id == null) &&
                          (x.OrderAssociatedPosition.Category3Id == null || x.OrderPrincipalPosition.Category3Id == null) &&
                          (x.OrderAssociatedPosition.FirmAddressId == x.OrderPrincipalPosition.FirmAddressId));
                }
            }
        }
    }
}