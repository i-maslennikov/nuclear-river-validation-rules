using System;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.ValidationRules.Replication.Specifications
{
    using Facts = Storage.Model.Facts;
    using Aggregates = Storage.Model.Aggregates;

    public static partial class Specs
    {
        public static partial class Map
        {
            public static class Facts
            {
                // ReSharper disable once InconsistentNaming
                public static class ToAggregates
                {
                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::Price>> Prices
                        = new MapSpecification<IQuery, IQueryable<Aggregates::Price>>(
                            q => q.For<Facts::Price>().Select(x => new Aggregates::Price
                                {
                                    Id = x.Id
                                }));

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::PriceDeniedPosition>> PriceDeniedPositions
                        = new MapSpecification<IQuery, IQueryable<Aggregates::PriceDeniedPosition>>(
                            q => q.For<Facts::DeniedPosition>().Select(x => new Aggregates::PriceDeniedPosition
                            {
                                PriceId = x.PriceId,
                                DeniedPositionId = x.PositionDeniedId,
                                PrincipalPositionId = x.PositionId,
                                ObjectBindingType = x.ObjectBindingType,
                            }));

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::PriceAssociatedPosition>> PriceAssociatedPositions
                        = new MapSpecification<IQuery, IQueryable<Aggregates::PriceAssociatedPosition>>(
                            q =>
                                {
                                    var aggs = from associatedPosition in q.For<Facts::AssociatedPosition>()
                                                join associatedPositionGroup in q.For<Facts::AssociatedPositionsGroup>() on associatedPosition.AssociatedPositionsGroupId equals associatedPositionGroup.Id
                                                join pricePosition in q.For<Facts::PricePosition>() on associatedPositionGroup.PricePositionId equals pricePosition.Id
                                                join price in q.For<Facts::Price>() on pricePosition.PriceId equals price.Id
                                                select new Aggregates::PriceAssociatedPosition
                                                {
                                                    PriceId = price.Id,
                                                    AssociatedPositionId = pricePosition.PositionId,
                                                    PrincipalPositionId = associatedPosition.PositionId,
                                                    ObjectBindingType = associatedPosition.ObjectBindingType,
                                                    GroupId = associatedPositionGroup.Id
                                                };
                                    return aggs;
                                });

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::AdvertisementAmountRestriction>> AdvertisementAmountRestrictions
                        = new MapSpecification<IQuery, IQueryable<Aggregates::AdvertisementAmountRestriction>>(
                            q => q.For<Facts::PricePosition>().Select(x => new Aggregates::AdvertisementAmountRestriction
                                {
                                    PriceId = x.PriceId,
                                    PositionId = x.PositionId,
                                    Max = x.MaxAdvertisementAmount,
                                    Min = x.MinAdvertisementAmount
                                }));

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::Ruleset>> Rulesets
                        = new MapSpecification<IQuery, IQueryable<Aggregates::Ruleset>>(
                            q =>
                            {
                                return q.For<Facts::RulesetRule>().OrderByDescending(x => x.Priority).Take(1).Select(x => new Aggregates::Ruleset
                                {
                                    Id = x.Id
                                });
                            });

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::RulesetRule>> RulesetRules
                        = new MapSpecification<IQuery, IQueryable<Aggregates::RulesetRule>>(
                            q =>
                                {
                                    var queгу = q.For<Facts::RulesetRule>();

                                    return queгу
                                            .Select(x => new
                                            {
                                                Rule = x,
                                                MaxPriority = queгу.Max(y => y.Priority)
                                            })
                                            .Where(x => x.Rule.Priority == x.MaxPriority)
                                            .Select(x => x.Rule)
                                            .Select(x => new Aggregates::RulesetRule
                                            {
                                                RulesetId = x.Id,
                                                RuleType = x.RuleType,

                                                DependentPositionId = x.DependentPositionId,
                                                PrincipalPositionId = x.PrincipalPositionId,
                                                ObjectBindingType = x.ObjectBindingType,
                                            });
                                });

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::Order>> Orders
                        = new MapSpecification<IQuery, IQueryable<Aggregates::Order>>(
                            q => q.For<Facts::Order>().Select(x => new Aggregates::Order
                                {
                                    Id = x.Id,
                                    FirmId = x.FirmId,
                                }));

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::OrderPosition>> OrderPositions
                        = new MapSpecification<IQuery, IQueryable<Aggregates::OrderPosition>>(
                            q =>
                                {
                                    var opas = from opa in q.For<Facts::OrderPositionAdvertisement>()
                                               join orderPosition in q.For<Facts::OrderPosition>() on opa.OrderPositionId equals orderPosition.Id
                                               select new Aggregates::OrderPosition
                                                {
                                                    OrderId = orderPosition.OrderId,
                                                    ItemPositionId = opa.PositionId,
                                                    CompareMode = (from position in q.For<Facts::Position>()
                                                                   where position.Id == opa.PositionId
                                                                   select position.CompareMode
                                                                   ).FirstOrDefault(),
                                                    Category3Id = opa.CategoryId,
                                                    FirmAddressId = opa.FirmAddressId,

                                                    PackagePositionId = (from pricePosition in q.For<Facts::PricePosition>()
                                                                        where pricePosition.Id == orderPosition.PricePositionId
                                                                        select pricePosition.PositionId
                                                                        ).FirstOrDefault(),
                                                    Category1Id = (from c3 in q.For<Facts::Category>()
                                                                    where c3.Id == opa.CategoryId
                                                                    join c2 in q.For<Facts::Category>() on c3.ParentId equals c2.Id
                                                                    join c1 in q.For<Facts::Category>() on c2.ParentId equals c1.Id
                                                                    select c1.Id
                                                                    ).FirstOrDefault()
                                                };

                                    var pkgs = from orderPosition in q.For<Facts::OrderPosition>()
                                               join pricePosition in q.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                                               join position in q.For<Facts::Position>() on pricePosition.PositionId equals position.Id
                                               where position.IsComposite
                                               select new Aggregates::OrderPosition
                                                {
                                                    OrderId = orderPosition.OrderId,
                                                    ItemPositionId = pricePosition.PositionId,
                                                    CompareMode = position.CompareMode,
                                                    Category3Id = null,
                                                    FirmAddressId = null,

                                                    PackagePositionId = pricePosition.PositionId,
                                                    Category1Id = null
                                                };

                                    return opas.Union(pkgs);
                                });

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::OrderPricePosition>> OrderPricePositions
                        = new MapSpecification<IQuery, IQueryable<Aggregates::OrderPricePosition>>(
                            q => from order in q.For<Facts::Order>()
                                 join orderPosition in q.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                                 join pricePosition in q.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                                 select new Aggregates::OrderPricePosition
                                     {
                                         OrderId = order.Id,
                                         OrderPositionId = orderPosition.Id,
                                         PriceId = pricePosition.PriceId
                                 });

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::Position>> Positions
                        = new MapSpecification<IQuery, IQueryable<Aggregates::Position>>(
                            q => q.For<Facts::Position>().Select(x => new Aggregates::Position
                                {
                                    Id = x.Id,
                                    CategoryCode = x.CategoryCode,
                                    IsControlledByAmount = x.IsControlledByAmount,
                                    Name = x.Name
                                }));

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::Period>> Periods
                        = new MapSpecification<IQuery, IQueryable<Aggregates::Period>>(
                            q =>
                                {
                                    var dates = q.For<Facts::Order>()
                                                 .Select(x => new { Date = x.BeginDistributionDate, OrganizationUnitId = x.DestOrganizationUnitId })
                                                 .Union(q.For<Facts::Order>().Select(x => new { Date = x.EndDistributionDateFact, OrganizationUnitId = x.DestOrganizationUnitId }))
                                                 .Union(q.For<Facts::Price>().Select(x => new { Date = x.BeginDate, x.OrganizationUnitId }))
                                                 .OrderBy(x => x.Date)
                                                 .Distinct();

                                    return dates.Select(x => new { start = x, end = dates.FirstOrDefault(y => y.Date > x.Date && y.OrganizationUnitId == x.OrganizationUnitId) })
                                                .Select(x => new Aggregates::Period
                                                    {
                                                        Start = x.start.Date,
                                                        End = x.end != null ? x.end.Date : DateTime.MaxValue,
                                                        OrganizationUnitId = x.start.OrganizationUnitId
                                                    });
                                });

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::OrderPeriod>> OrderPeriods
                        = new MapSpecification<IQuery, IQueryable<Aggregates::OrderPeriod>>(
                            q =>
                            {
                                var dates = q.For<Facts::Order>()
                                             .Select(x => new { Date = x.BeginDistributionDate, OrganizationUnitId = x.DestOrganizationUnitId })
                                             .Union(q.For<Facts::Order>().Select(x => new { Date = x.EndDistributionDateFact, OrganizationUnitId = x.DestOrganizationUnitId }))
                                             .Union(q.For<Facts::Price>().Select(x => new { Date = x.BeginDate, x.OrganizationUnitId }))
                                             .Distinct();

                                // https://github.com/linq2db/linq2db/issues/356
                                dates = dates.Select(x => new { x.Date, x.OrganizationUnitId });

                                var result = q.For<Facts::Order>()
                                              .SelectMany(order => dates.Where(date =>
                                                                               date.OrganizationUnitId == order.DestOrganizationUnitId &&
                                                                               order.BeginDistributionDate <= date.Date && date.Date < order.EndDistributionDateFact)
                                                                        .Select(x => new Aggregates::OrderPeriod
                                                                            {
                                                                                OrderId = order.Id,
                                                                                OrganizationUnitId = order.DestOrganizationUnitId,
                                                                                Start = x.Date
                                                                            }));

                                return result;
                            });

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::PricePeriod>> PricePeriods
                        = new MapSpecification<IQuery, IQueryable<Aggregates::PricePeriod>>(
                            q =>
                                {
                                    var dates = q.For<Facts::Order>()
                                                 .Select(x => new { Date = x.BeginDistributionDate, OrganizationUnitId = x.DestOrganizationUnitId })
                                                 .Union(q.For<Facts::Order>().Select(x => new { Date = x.EndDistributionDateFact, OrganizationUnitId = x.DestOrganizationUnitId }))
                                                 .Union(q.For<Facts::Price>().Select(x => new { Date = x.BeginDate, x.OrganizationUnitId }))
                                                 .Distinct();

                                    var result = dates.Select(date => new
                                                                    {
                                                                        PriceId = (long?)q.For<Facts::Price>()
                                                                                            .Where(price => price.OrganizationUnitId == date.OrganizationUnitId && price.BeginDate <= date.Date)
                                                                                            .OrderByDescending(price => price.BeginDate)
                                                                                            .FirstOrDefault()
                                                                                            .Id,
                                                                        Period = date
                                                                    })
                                                      .Where(x => x.PriceId.HasValue)
                                                      .Select(x => new Aggregates::PricePeriod
                                                                    {
                                                                        OrganizationUnitId = x.Period.OrganizationUnitId,
                                                                        PriceId = x.PriceId.Value,
                                                                        Start = x.Period.Date
                                                                    });

                                    return result;
                                });
                }
            }
        }
    }
}