using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.ValidationRules.Replication.PriceRules.Validation.Dto;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Replication.Specifications
{
    public static partial class Specs
    {
        public static partial class Join
        {
            public static class Aggs
            {
                public static Expression<Func<Dto<OrderAssociatedPosition>, IEnumerable<Dto<OrderPosition>>>> WithMatchedBindingObject(IQueryable<Dto<OrderPosition>> principals)
                {
                    return associated => principals.Where(principal => principal.FirmId == associated.FirmId
                                                                       && principal.Start == associated.Start
                                                                       && principal.OrganizationUnitId == associated.OrganizationUnitId
                                                                       && principal.Position.ItemPositionId == associated.Position.PrincipalPositionId
                                                                       && principal.Position.OrderPositionId != associated.Position.CauseOrderPositionId
                                                                       && (principal.Scope == 0 || principal.Scope == associated.Scope))
                                                   .Where(principal =>
                                                          (associated.Position.HasNoBinding == principal.Position.HasNoBinding) &&
                                                          ((associated.Position.Category1Id == principal.Position.Category1Id) &&
                                                           (associated.Position.Category3Id == principal.Position.Category3Id || associated.Position.Category3Id == null || principal.Position.Category3Id == null) &&
                                                           (associated.Position.FirmAddressId == principal.Position.FirmAddressId || associated.Position.FirmAddressId == null || principal.Position.FirmAddressId == null) ||
                                                           (associated.Position.Category1Id == principal.Position.Category1Id || associated.Position.Category1Id == null || principal.Position.Category1Id == null) && (associated.Position.Category3Id == null || principal.Position.Category3Id == null) &&
                                                           (associated.Position.FirmAddressId == principal.Position.FirmAddressId)));
                }
            }
        }
    }
}