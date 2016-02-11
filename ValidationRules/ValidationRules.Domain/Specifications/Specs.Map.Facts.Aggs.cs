using System;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.ValidationRules.Domain.Specifications
{
    using Facts = Model.Facts;
    using Aggregates = Model.Aggregates;

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

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::DeniedPosition>> DeniedPositions
                        = new MapSpecification<IQuery, IQueryable<Aggregates::DeniedPosition>>(
                            q =>
                                {
                                    var deniedPositions = q.For<Facts::DeniedPosition>().Select(x => new
                                        {
                                            x.PositionId,
                                            DeniedPositionId = x.PositionDeniedId,
                                            x.ObjectBindingType,
                                            PriceId = (long?)x.PriceId,
                                        });

                                    var globalDeniedPositions = q.For<Facts::GlobalDeniedPosition>().Select(x => new
                                        {
                                            PositionId = x.MasterPositionId,
                                            x.DeniedPositionId,
                                            x.ObjectBindingType,
                                            PriceId = (long?)null,
                                        });

                                    var aggs = deniedPositions.Union(globalDeniedPositions).Select(x => new Aggregates::DeniedPosition
                                        {
                                            PositionId = x.PositionId,
                                            DeniedPositionId = x.DeniedPositionId,
                                            PriceId = x.PriceId,
                                            ObjectBindingType = x.ObjectBindingType
                                        });

                                    return aggs;
                                });

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::MasterPosition>> MasterPositions
                        = new MapSpecification<IQuery, IQueryable<Aggregates::MasterPosition>>(
                            q =>
                                {
                                    var masterPositions = from associatedPosition in q.For<Facts::AssociatedPosition>()
                                                          join associatedPositionGroup in q.For<Facts::AssociatedPositionsGroup>() on associatedPosition.AssociatedPositionsGroupId
                                                              equals associatedPositionGroup.Id
                                                          join pricePosition in q.For<Facts::PricePosition>() on associatedPositionGroup.PricePositionId equals pricePosition.Id
                                                          join price in q.For<Facts::Price>() on pricePosition.PriceId equals price.Id
                                                          select new Aggregates::MasterPosition
                                                              {
                                                                  PositionId = pricePosition.PositionId,
                                                                  MasterPositionId = associatedPosition.PositionId,
                                                                  ObjectBindingType = associatedPosition.ObjectBindingType,

                                                                  PriceId = price.Id,
                                                                  GroupId = associatedPositionGroup.Id
                                                              };

                                    var globalMasterPositions = q.For<Facts::GlobalAssociatedPosition>().Select(x => new Aggregates::MasterPosition
                                        {
                                            PositionId = x.AssociatedPositionId,
                                            MasterPositionId = x.MasterPositionId,
                                            ObjectBindingType = x.ObjectBindingType,

                                            PriceId = null,
                                            GroupId = null
                                        });

                                    return masterPositions.Union(globalMasterPositions);
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
                                               join pricePosition in q.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                                               select new Aggregates::OrderPosition
                                                   {
                                                       OrderId = orderPosition.OrderId,
                                                       PackagePositionId = pricePosition.PositionId,
                                                       ItemPositionId = opa.PositionId,
                                                       CategoryId = opa.CategoryId,
                                                       FirmAddressId = opa.FirmAddressId
                                                   };

                                    var pkgs = from orderPosition in q.For<Facts::OrderPosition>()
                                               join pricePosition in q.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                                               join position in q.For<Facts::Position>() on pricePosition.PositionId equals position.Id
                                               where position.IsComposite
                                               select new Aggregates::OrderPosition
                                                   {
                                                       OrderId = orderPosition.OrderId,
                                                       PackagePositionId = pricePosition.PositionId,
                                                       ItemPositionId = pricePosition.PositionId,
                                                       CategoryId = null,
                                                       FirmAddressId = null
                                                   };

                                    return opas.Union(pkgs);
                                });

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::OrderPrice>> OrderPrices
                        = new MapSpecification<IQuery, IQueryable<Aggregates::OrderPrice>>(
                            q =>
                                {
                                    var orderPrices = from order in q.For<Facts::Order>()
                                                      join orderPosition in q.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                                                      join pricePosition in q.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                                                      select new Aggregates::OrderPrice
                                                          {
                                                              OrderId = order.Id,
                                                              PriceId = pricePosition.PriceId
                                                          };

                                    return orderPrices.Distinct();
                                });

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::Position>> Positions
                        = new MapSpecification<IQuery, IQueryable<Aggregates::Position>>(
                            q => q.For<Facts::Position>().Select(x => new Aggregates::Position
                                {
                                    Id = x.Id,
                                    PositionCategoryId = x.PositionCategoryId
                                }));

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::Period>> Periods
                        = new MapSpecification<IQuery, IQueryable<Aggregates::Period>>(
                            q =>
                                {
                                    var dates = q.For<Facts::Order>()
                                                 .Select(x => new { Date = x.BeginDistributionDate, OrganizationUnitId = x.DestOrganizationUnitId })
                                                 .Union(q.For<Facts::Order>().Select(x => new { Date = x.EndDistributionDateFact, OrganizationUnitId = x.DestOrganizationUnitId }))
                                                 .Union(q.For<Facts::Price>().Select(x => new { Date = x.BeginDate, OrganizationUnitId = x.OrganizationUnitId }))
                                                 .Select(x => new { x.Date, x.OrganizationUnitId })
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
                }
            }
        }
    }
}