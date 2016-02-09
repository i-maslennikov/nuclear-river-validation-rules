using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Domain.Model.Erm;

namespace NuClear.ValidationRules.Domain.Specifications
{
    using Facts = Model.Facts;
    using Aggs = Model.Aggregates;

    public static partial class Specs
    {
        public static partial class Map
        {
            public static class Facts
            {
                // ReSharper disable once InconsistentNaming
                public static class ToAggregates
                {
                    public static readonly MapSpecification<IQuery, IQueryable<Aggs::Price>> Prices = new MapSpecification<IQuery, IQueryable<Aggs::Price>>(q =>
                        q.For<Facts::Price>().Select(x => new Aggs::Price
                        {
                                Id = x.Id
                        }));

                    public static readonly MapSpecification<IQuery, IQueryable<Aggs::DeniedPosition>> DeniedPositions = new MapSpecification<IQuery, IQueryable<Aggs::DeniedPosition>>(q =>
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

                        var aggs = deniedPositions.Union(globalDeniedPositions).Select(x => new Aggs::DeniedPosition
                        {
                            PositionId = x.PositionId,
                            DeniedPositionId = x.DeniedPositionId,
                            PriceId = x.PriceId,
                            ObjectBindingType = x.ObjectBindingType
                        });

                        return aggs;
                    });

                    public static readonly MapSpecification<IQuery, IQueryable<Aggs::MasterPosition>> MasterPositions = new MapSpecification<IQuery, IQueryable<Aggs::MasterPosition>>(q =>
                    {
                        var masterPositions = from associatedPosition in q.For<Facts::AssociatedPosition>()
                                              join associatedPositionGroup in q.For<Facts::AssociatedPositionsGroup>() on associatedPosition.AssociatedPositionsGroupId equals associatedPositionGroup.Id
                                              join pricePosition in q.For<Facts::PricePosition>() on associatedPositionGroup.PricePositionId equals pricePosition.Id
                                              join price in q.For<Facts::Price>() on pricePosition.PriceId equals price.Id
                                              select new Aggs::MasterPosition
                                              {
                                                  PositionId = pricePosition.PositionId,
                                                  MasterPositionId = associatedPosition.PositionId,
                                                  ObjectBindingType = associatedPosition.ObjectBindingType,

                                                  PriceId = price.Id,
                                                  GroupId = associatedPositionGroup.Id
                                              };

                        var globalMasterPositions = q.For<Facts::GlobalAssociatedPosition>().Select(x => new Aggs::MasterPosition
                        {
                            PositionId = x.AssociatedPositionId,
                            MasterPositionId = x.MasterPositionId,
                            ObjectBindingType = x.ObjectBindingType,

                            PriceId = null,
                            GroupId = null
                        });

                        return masterPositions.Union(globalMasterPositions);
                    });

                    public static readonly MapSpecification<IQuery, IQueryable<Aggs::AdvertisementAmountRestriction>> AdvertisementAmountRestrictions = new MapSpecification<IQuery, IQueryable<Aggs::AdvertisementAmountRestriction>>(q =>
                        q.For<Facts::PricePosition>().Select(x => new Aggs::AdvertisementAmountRestriction
                        {
                            PriceId = x.PriceId,
                            PositionId = x.PositionId,
                            Max = x.MaxAdvertisementAmount,
                            Min = x.MinAdvertisementAmount
                        }));

                    public static readonly MapSpecification<IQuery, IQueryable<Aggs::Order>> Orders = new MapSpecification<IQuery, IQueryable<Aggs::Order>>(q =>
                        q.For<Facts::Order>().Select(x => new Aggs::Order
                        {
                            Id = x.Id,
                            FirmId = x.FirmId,
                        }));

                    public static readonly MapSpecification<IQuery, IQueryable<Aggs::OrderPosition>> OrderPositions = new MapSpecification<IQuery, IQueryable<Aggs::OrderPosition>>(q =>
                    {
                        var opas = from opa in q.For<Facts::OrderPositionAdvertisement>()
                                   join orderPosition in q.For<Facts::OrderPosition>() on opa.OrderPositionId equals orderPosition.Id
                                   select new Aggs::OrderPosition
                                   {
                                        OrderId = orderPosition.OrderId,
                                        PackagePositionId = null,
                                        ItemPositionId = opa.PositionId,
                                        CategoryId = opa.CategoryId,
                                        FirmAddressId = opa.FirmAddressId
                                   };

                        var pkgs = from orderPosition in q.For<Facts::OrderPosition>()
                                   join pricePosition in q.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                                   join position in q.For<Facts::Position>() on pricePosition.PositionId equals position.Id
                                   where position.IsComposite
                                   select new Aggs::OrderPosition
                                   {
                                        OrderId = orderPosition.OrderId,
                                        PackagePositionId = position.Id,
                                        ItemPositionId = position.Id,
                                        CategoryId = null,
                                        FirmAddressId = null
                                   };

                        return opas.Union(pkgs);
                    });

                    public static readonly MapSpecification<IQuery, IQueryable<Aggs::OrderPrice>> OrderPrices = new MapSpecification<IQuery, IQueryable<Aggs::OrderPrice>>(q =>
                    {
                        var orderPrices = from order in q.For<Facts::Order>()
                                          join orderPosition in q.For<Facts::OrderPosition>() on order.Id equals orderPosition.OrderId
                                          join pricePosition in q.For<Facts::PricePosition>() on orderPosition.PricePositionId equals pricePosition.Id
                                          select new Aggs::OrderPrice
                                          {
                                              OrderId = order.Id,
                                              PriceId = pricePosition.PriceId
                                          };

                        return orderPrices.Distinct();
                    });

                    public static readonly MapSpecification<IQuery, IQueryable<Aggs::Position>> Positions = new MapSpecification<IQuery, IQueryable<Aggs::Position>>(q =>
                        q.For<Facts::Position>().Select(x => new Aggs::Position
                        {
                            Id = x.Id,
                            PositionCategoryId = x.PositionCategoryId
                        }));
                }
            }
        }
    }
}