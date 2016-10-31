using System;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.ValidationRules.Replication.Specifications
{
    using Facts = Storage.Model.PriceRules.Facts;
    using Aggregates = Storage.Model.PriceRules.Aggregates;

    public static partial class Specs
    {
        public static partial class Map
        {
            public static class Facts
            {
                // ReSharper disable once InconsistentNaming
                public static class ToAggregates
                {
                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::Order>> Orders
                        = new MapSpecification<IQuery, IQueryable<Aggregates::Order>>(
                            q => q.For<Facts::Order>().Select(x => new Aggregates::Order
                                {
                                    Id = x.Id,
                                    FirmId = x.FirmId,
                                    Number = x.Number,
                                }));

                    public static readonly MapSpecification<IQuery, IQueryable<Aggregates::Position>> Positions
                        = new MapSpecification<IQuery, IQueryable<Aggregates::Position>>(
                            q => q.For<Facts::Position>().Select(x => new Aggregates::Position
                                {
                                    Id = x.Id,
                                    CategoryCode = x.CategoryCode,
                                    Name = x.Name
                                }));
                }
            }
        }
    }
}