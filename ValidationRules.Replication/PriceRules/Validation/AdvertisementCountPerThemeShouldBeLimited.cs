using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказов и проектов, с превышением числа проданных тематик, должна выводиться ошибка.
    /// "Слишком много продаж в тематику {0}. Продано {1} позиций вместо {2} возможных"
    /// 
    /// Source: ThemePositionCountValidationRule
    /// </summary>
    public sealed class AdvertisementCountPerThemeShouldBeLimited : ValidationResultAccessorBase
    {
        private const int MaxPositionsPerTheme = 10;

        public AdvertisementCountPerThemeShouldBeLimited(IQuery query) : base(query, MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            // todo: Тематики больше не продаются, проверка не оттестирована тщательно, вероятно скоро можно будет удалить совсем.
            var sales =
                from orderPosition in query.For<Order.OrderThemePosition>()
                from orderPeriod in query.For<Order.OrderPeriod>().Where(x => x.OrderId == orderPosition.OrderId)
                from period in query.For<Period>().Where(x => orderPeriod.Begin <= x.Start && x.End <= orderPeriod.End && x.ProjectId == orderPosition.ProjectId)
                select new { orderPosition.ThemeId, orderPosition.ProjectId, orderPeriod.OrderId, orderPeriod.Scope, period.Start, period.End };

            var oversales =
                from sale in sales
                let count = sales
                    .Count(x => x.ThemeId == sale.ThemeId
                                && x.ProjectId == sale.ProjectId
                                && x.Start == sale.Start
                                && Scope.CanSee(sale.Scope, x.Scope))
                where count > MaxPositionsPerTheme
                select new { sale.Start, sale.End, sale.ProjectId, sale.OrderId, sale.ThemeId, Count = count };

            var messages =
                from oversale in oversales
                select new Version.ValidationResult
                {
                    MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "max", MaxPositionsPerTheme }, { "count", oversale.Count } },
                                    new Reference<EntityTypeTheme>(oversale.ThemeId),
                                    new Reference<EntityTypeProject>(oversale.ProjectId))
                                .ToXDocument(),

                    PeriodStart = oversale.Start,
                    PeriodEnd = oversale.End,
                    OrderId = oversale.OrderId,
                };

            return messages;
        }
    }
}
