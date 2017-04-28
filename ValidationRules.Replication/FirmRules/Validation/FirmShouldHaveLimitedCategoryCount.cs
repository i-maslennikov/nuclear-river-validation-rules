using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.FirmRules.Validation
{
    /// <summary>
    /// Для фирм, которые одновременно размещаются более чем в N рубриках, должно выводиться предупреждение.
    /// "Для фирмы {0} задано слишком большое число рубрик - {1}. Максимально допустимое - {2}"
    /// 
    /// Source: CategoriesForFirmAmountOrderValidationRule
    /// 
    /// Q: Что если у фирмы 20 рубрик в одном заказе, который в статусе на расторжении и ещё одна рубрика в заказе, который начинает размещение с даты расторжения (и пересекается по датам с первым)
    /// A: Проверка не срабатывает
    /// </summary>
    public sealed class FirmShouldHaveLimitedCategoryCount : ValidationResultAccessorBase
    {
        // todo: можно ограничить эту проверку идентификаторами заказа, что даст выигрыш до полутора секунд.
        private const int MaxCategoriesAlowedForFirm = 20;

        public FirmShouldHaveLimitedCategoryCount(IQuery query) : base(query, MessageTypeCode.FirmShouldHaveLimitedCategoryCount)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var firmPeriods =
                query.For<Firm.CategoryPurchase>().Select(purchase => new { purchase.FirmId, purchase.Begin, purchase.End }).Distinct();

            var oversales =
                from order in query.For<Order>()
                from period in firmPeriods.Where(x => x.FirmId == order.FirmId && order.Begin <= x.Begin && x.End <= order.End)
                let count = query.For<Firm.CategoryPurchase>()
                                 .Where(x => x.FirmId == period.FirmId && x.Begin == period.Begin && x.End == period.End && Scope.CanSee(order.Scope, x.Scope))
                                 .Select(x => x.CategoryId)
                                 .Distinct()
                                 .Count()
                where count > MaxCategoriesAlowedForFirm
                select new { OrderId = order.Id, order.FirmId, period.Begin, period.End, Count = count };

            var messages =
                from oversale in oversales
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "count", oversale.Count }, { "allowed", MaxCategoriesAlowedForFirm } },
                                    new Reference<EntityTypeFirm>(oversale.FirmId))
                                .ToXDocument(),

                        PeriodStart = oversale.Begin,
                        PeriodEnd = oversale.End,
                        OrderId = oversale.OrderId,
                    };

            return messages;
        }
    }
}
