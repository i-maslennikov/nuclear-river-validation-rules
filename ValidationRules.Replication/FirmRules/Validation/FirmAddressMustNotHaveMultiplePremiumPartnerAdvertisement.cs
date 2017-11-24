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
    /// Для заказов, при наличии нескольких позиций премиумной партнёрской рекламы (ЗМК-Premium подобные, FMCG) на один адрес, должна выводиться ошибка.
    /// "На адрес {0} фирмы {1} продано более одной кнопки в заголовок карточки в периоды: {2}"
    /// </summary>
    public sealed class FirmAddressMustNotHaveMultiplePremiumPartnerAdvertisement : ValidationResultAccessorBase
    {
        public FirmAddressMustNotHaveMultiplePremiumPartnerAdvertisement(IQuery query) : base(query, MessageTypeCode.FirmAddressMustNotHaveMultiplePremiumPartnerAdvertisement)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var sales =
                from order in query.For<Order>()
                from fa in query.For<Order.PartnerPosition>().Where(x => x.IsPremium).Where(x => x.OrderId == order.Id)
                select new { fa.OrderId, FirmAddressId = fa.DestinationFirmAddressId, FirmId = fa.DestinationFirmId, order.Scope, order.Begin, order.End };

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
                                    new Dictionary<string, object> { { "begin", sale.Begin }, { "end", sale.End } },
                                    new Reference<EntityTypeOrder>(sale.OrderId),
                                    new Reference<EntityTypeFirm>(sale.FirmId),
                                    new Reference<EntityTypeFirmAddress>(sale.FirmAddressId))
                                .ToXDocument(),

                        PeriodStart = sale.Begin,
                        PeriodEnd = sale.End,
                        OrderId = sale.OrderId,
                    };

            return messages;
        }
    }
}
