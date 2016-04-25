using System;
using System.Linq;
using System.Collections.Generic;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Domain.Model;

namespace NuClear.ValidationRules.Domain.Specifications
{
    using Facts = Model.Facts;

    public static partial class Specs
    {
        public static partial class Map
        {
            public static partial class Facts
            {
                public static class ToOrderAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<long>> ByCategory(FindSpecification<Facts::Category> specification)
                    {
                        // todo: А нужно ли пересчитывать заказы, которые имею продажи в одну из родительских рубрик?
                        // т.е. связаны не с Category.Id, а с Category.ParentId (по факту не бывает) или Category.ParentId.ParentId (очень даже бывает)
                        // нет, не требуется - это должно быть поддержано на уровне вычисления сообщений, а для пересчёта заказа не требуется.
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from category in q.For(specification)
                                  join opa in q.For<Facts::OrderPositionAdvertisement>() on category.Id equals opa.CategoryId
                                  join orderPosition in q.For<Facts::OrderPosition>() on opa.OrderPositionId equals orderPosition.Id
                                  select orderPosition.OrderId).Distinct()
                        );
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByOrderPosition(FindSpecification<Facts::OrderPosition> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from orderPosition in q.For(specification)
                                  select orderPosition.OrderId).Distinct()
                        );
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByOrderPositionAdvertisement(FindSpecification<Facts::OrderPositionAdvertisement> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from opa in q.For(specification)
                                  join orderPosition in q.For<Facts::OrderPosition>() on opa.OrderPositionId equals orderPosition.Id
                                  select orderPosition.OrderId).Distinct()
                        );
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByPosition(FindSpecification<Facts::Position> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q =>
                                {
                                    var viaOrderPositionAdvertisement
                                        = from position in q.For(specification)
                                          join opa in q.For<Facts::OrderPositionAdvertisement>() on position.Id equals opa.PositionId
                                          join orderPosition in q.For<Facts::OrderPosition>() on opa.OrderPositionId equals orderPosition.Id
                                          select orderPosition.OrderId;

                                    var viaPricePosition
                                        = from position in q.For(specification)
                                           join pp in q.For<Facts::PricePosition>() on position.Id equals pp.PositionId
                                           join orderPosition in q.For<Facts::OrderPosition>() on pp.Id equals orderPosition.PricePositionId
                                           select orderPosition.OrderId;

                                    return viaOrderPositionAdvertisement.Concat(viaPricePosition).Distinct();
                                }
                            );
                    }
                }

                public static class ToPriceAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<long>> ByAssociatedPosition(FindSpecification<Facts::AssociatedPosition> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from associatedPosition in q.For(specification)
                                 join associatedPositionsGroup in q.For<Facts::AssociatedPositionsGroup>() on associatedPosition.AssociatedPositionsGroupId equals associatedPositionsGroup.Id
                                 join pricePosition in q.For<Facts::PricePosition>() on associatedPositionsGroup.PricePositionId equals pricePosition.Id
                                 select pricePosition.PriceId).Distinct()
                        );
                    }
                    public static MapSpecification<IQuery, IEnumerable<long>> ByAssociatedPositionGroup(FindSpecification<Facts::AssociatedPositionsGroup> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from associatedPositionsGroup in q.For(specification)
                                  join pricePosition in q.For<Facts::PricePosition>() on associatedPositionsGroup.PricePositionId equals pricePosition.Id
                                  select pricePosition.PriceId).Distinct()
                        );
                    }
                    public static MapSpecification<IQuery, IEnumerable<long>> ByDeniedPosition(FindSpecification<Facts::DeniedPosition> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from deniedPosition in q.For(specification)
                                  select deniedPosition.PriceId).Distinct()
                        );
                    }

                    public static MapSpecification<IQuery, IEnumerable<long>> ByPricePosition(FindSpecification<Facts::PricePosition> specification)
                    {
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from pricePosition in q.For(specification)
                                  select pricePosition.PriceId).Distinct()
                        );
                    }
                }

                public static class ToRulesetAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<long>> ByRulesetRule(FindSpecification<Facts::RulesetRule> specification)
                    {
                        // always recalculate all rulesets
                        return new MapSpecification<IQuery, IEnumerable<long>>(
                            q => (from rulesetRule in q.For<Facts::RulesetRule>()
                                  select rulesetRule.Id).Distinct()
                        );
                    }
                }

                public static class ToPeriodAggregate
                {
                    public static MapSpecification<IQuery, IEnumerable<PeriodKey>> ByPrice(FindSpecification<Facts::Price> specification)
                        => new MapSpecification<IQuery, IEnumerable<PeriodKey>>(
                            q =>
                                {
                                    var ranges = q.For<Facts::Price>()
                                                  .Where(specification)
                                                  .GroupBy(x => x.OrganizationUnitId, x => x.BeginDate)
                                                  .ToDictionary(x => x.Key, x => x.Min());

                                    var dates = q.For<Facts::Order>()
                                                 .Select(x => new { Date = x.BeginDistributionDate, OrganizationUnitId = x.DestOrganizationUnitId })
                                                 .Union(q.For<Facts::Order>().Select(x => new { Date = x.EndDistributionDateFact, OrganizationUnitId = x.DestOrganizationUnitId }))
                                                 .Union(q.For<Facts::Price>().Select(x => new { Date = x.BeginDate, OrganizationUnitId = x.OrganizationUnitId }))
                                                 .GroupBy(x => x.OrganizationUnitId, x => x.Date)
                                                 .ToDictionary(x => x.Key, x => x.Distinct());

                                    var result = ranges.Join(dates,
                                                             x => x.Key,
                                                             x => x.Key,
                                                             (range, date) => date.Value
                                                                                  .Where(d => range.Value <= d)
                                                                                  .Select(d => new PeriodKey { OrganizationUnitId = range.Key, Start = d }))
                                                       .SelectMany(x => x)
                                                       .Distinct(new PeriodKeyEqualityComparer());

                                    return result;
                                }
                            );

                    public static MapSpecification<IQuery, IEnumerable<PeriodKey>> ByOrder(FindSpecification<Facts::Order> specification)
                        => new MapSpecification<IQuery, IEnumerable<PeriodKey>>(
                            q =>
                                {
                                    var ranges = q.For<Facts::Order>()
                                                  .Where(specification)
                                                  .GroupBy(x => x.DestOrganizationUnitId, x => new { Start = x.BeginDistributionDate, End = x.EndDistributionDateFact })
                                                  .ToDictionary(x => x.Key, x => x.Distinct());

                                    var dates = q.For<Facts::Order>()
                                                 .Select(x => new { Date = x.BeginDistributionDate, OrganizationUnitId = x.DestOrganizationUnitId })
                                                 .Union(q.For<Facts::Order>().Select(x => new { Date = x.EndDistributionDateFact, OrganizationUnitId = x.DestOrganizationUnitId }))
                                                 .Union(q.For<Facts::Price>().Select(x => new { Date = x.BeginDate, OrganizationUnitId = x.OrganizationUnitId }))
                                                 .GroupBy(x => x.OrganizationUnitId, x => x.Date)
                                                 .ToDictionary(x => x.Key, x => x.Distinct());

                                    var result = ranges.Join(dates,
                                                             x => x.Key,
                                                             x => x.Key,
                                                             (range, date) => date.Value
                                                                                  .Where(d => range.Value.Any(r => r.Start <= d && d <= r.End))
                                                                                  .Select(d => new PeriodKey { OrganizationUnitId = range.Key, Start = d }))
                                                       .SelectMany(x => x)
                                                       .Distinct(new PeriodKeyEqualityComparer());

                                    return result;
                                }
                            );

                    private class PeriodKeyEqualityComparer : IEqualityComparer<PeriodKey>
                    {
                        public bool Equals(PeriodKey x, PeriodKey y)
                        {
                            return x.OrganizationUnitId == y.OrganizationUnitId && x.Start == y.Start;
                        }

                        public int GetHashCode(PeriodKey obj)
                        {
                            return obj.OrganizationUnitId.GetHashCode() ^ obj.Start.GetHashCode();
                        }
                    }
                }
            }
        }
    }
}