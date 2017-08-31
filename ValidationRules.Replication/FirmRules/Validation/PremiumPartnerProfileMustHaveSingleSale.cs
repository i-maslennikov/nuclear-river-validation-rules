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
    public sealed class PremiumPartnerProfileMustHaveSingleSale : ValidationResultAccessorBase
    {
        public PremiumPartnerProfileMustHaveSingleSale(IQuery query) : base(query, MessageTypeCode.PremiumPartnerProfileMustHaveSingleSale)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var sales =
                from order in query.For<Order>()
                from fa in query.For<Order.PremiumPartnerProfilePosition>().Where(x => x.OrderId == order.Id)
                select new { fa.OrderId, fa.FirmAddressId, fa.FirmId, order.Scope, order.Begin, order.End };

            var multipleSales =
                from sale in sales
                from conflict in sales.Where(x => x.FirmAddressId == sale.FirmAddressId && x.OrderId != sale.OrderId)
                where sale.Begin < conflict.End && conflict.Begin < sale.End && Scope.CanSee(sale.Scope, conflict.Scope)
                select new { sale.OrderId, sale.FirmAddressId, sale.FirmId, Begin = sale.Begin < conflict.Begin ? conflict.Begin : sale.Begin, End = sale.End < conflict.End ? sale.End : conflict.End };

            var messages =
                from sale in multipleSales
                select new Version.ValidationResult
                {
                    MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeFirm>(sale.FirmId),
                                    new Reference<EntityTypeFirm>(sale.FirmAddressId))
                                .ToXDocument(),

                    PeriodStart = sale.Begin,
                    PeriodEnd = sale.End,
                    OrderId = sale.OrderId,
                };

            return messages;
        }
    }
}
