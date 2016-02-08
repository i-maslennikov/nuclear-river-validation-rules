using System.Linq;

using NuClear.ValidationRules.Domain.Model.Aggregates;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

using Facts = NuClear.ValidationRules.Domain.Model.Facts;

namespace NuClear.ValidationRules.Domain.Specifications
{
    public static partial class Specs
    {
        public static partial class Map
        {
            public static partial class Facts
            {
                // ReSharper disable once InconsistentNaming
                public static class ToAggregates
                {
                    public static readonly MapSpecification<IQuery, IQueryable<Price>> Prices = new MapSpecification<IQuery, IQueryable<Price>>(
                        q => q.For<Facts::Price>().Select(x => new Price
                            {
                                Id = x.Id
                            })
                        );

                    public static readonly MapSpecification<IQuery, IQueryable<DeniedPosition>> DeniedPositions = new MapSpecification<IQuery, IQueryable<DeniedPosition>>(
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

                                var aggs = deniedPositions.Union(globalDeniedPositions).Select(x => new DeniedPosition
                                {
                                        PositionId = x.PositionId,
                                        DeniedPositionId = x.DeniedPositionId,
                                        PriceId = x.PriceId,
                                        ObjectBindingType = x.ObjectBindingType
                                });

                                return aggs;
                            }
                        );
                }
            }
        }
    }
}