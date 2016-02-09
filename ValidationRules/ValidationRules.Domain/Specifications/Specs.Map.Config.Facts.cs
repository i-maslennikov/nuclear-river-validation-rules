using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Specifications;

namespace NuClear.ValidationRules.Domain.Specifications
{
    using Dto = Dto;
    using Facts = Model.Facts;

    public static partial class Specs
    {
        public static partial class Map
        {
            public static class Config
            {
                public static class ToFacts
                {
                    public static readonly MapSpecification<Dto::OrderValidationConfig, IReadOnlyCollection<Facts::GlobalAssociatedPosition>> GlobalAssociatedPosition =
                        new MapSpecification<Dto::OrderValidationConfig, IReadOnlyCollection<Facts::GlobalAssociatedPosition>>(
                            config => config.Positions.SelectMany(position => position.MasterPositions.Select(master => new Facts::GlobalAssociatedPosition
                            {
                                    MasterPositionId = master.Id,
                                    AssociatedPositionId = position.Id,
                                    ObjectBindingType = Parce(master.BindingType),
                                })).ToArray());

                    // todo: в denied positions должна быть избыточность, симметричные пары тоже должны импортироваться
                    public static readonly MapSpecification<Dto::OrderValidationConfig, IReadOnlyCollection<Facts::GlobalDeniedPosition>> GlobalDeniedPosition =
                        new MapSpecification<Dto::OrderValidationConfig, IReadOnlyCollection<Facts::GlobalDeniedPosition>>(
                            config => config.Positions.SelectMany(position => position.DeniedPositions.Select(denied => new Facts::GlobalDeniedPosition
                            {
                                MasterPositionId = position.Id,
                                DeniedPositionId = denied.Id,
                                ObjectBindingType = Parce(denied.BindingType),
                            })).ToArray());

                    private static int Parce(string bindingType)
                    {
                        switch (bindingType.ToLowerInvariant())
                        {
                            case "match":
                                return 1;
                            case "nodependency":
                                return 2;
                            case "different":
                                return 3;
                            default:
                                throw new ArgumentException($"Unknown binding type {bindingType}", nameof(bindingType));
                        }
                    }
                }
            }
        }
    }
}