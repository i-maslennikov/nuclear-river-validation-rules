using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.FirmRules.Validation
{
    /// <summary>
    /// Для заказов, в адресах фирмы которых размещаются позиции ЗМК, должно быть предупреждение.
    /// "На адрес {0} фирмы {1} продана позиция ЗМК в заказе {2}"
    /// </summary>
    public sealed class FirmAddressMayBeSharedWithPartner : ValidationResultAccessorBase
    {
        public FirmAddressMayBeSharedWithPartner(IQuery query) : base(query, MessageTypeCode.FirmAddressMayBeSharedWithPartner)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var sales =
                from order in query.For<Order>()
                from fa in query.For<Order.PartnerProfilePosition>().Where(x => x.OrderId == order.Id)
                from partnerOrder in query.For<Order>().Where(x => x.FirmId == fa.FirmId)
                                          .Where(x => x.Begin < order.End && order.Begin < x.End)
                select new // Здесь: order - содержит ЗМК, partnerOrder - заказ фирмы, которой подкинули рекламу
                {
                        OrderId = order.Id,
                        fa.FirmId,
                        fa.FirmAddressId,

                        PartnerOrderId = partnerOrder.Id,
                        Begin = order.Begin < partnerOrder.Begin ? partnerOrder.Begin : order.Begin,
                        End = order.End < partnerOrder.End ? order.End : partnerOrder.End,
                    };

            var messages =
                from sale in sales
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                              new Reference<EntityTypeOrder>(sale.PartnerOrderId),
                                              new Reference<EntityTypeOrder>(sale.OrderId),
                                              new Reference<EntityTypeFirm>(sale.FirmId),
                                              new Reference<EntityTypeFirmAddress>(sale.FirmAddressId))
                                .ToXDocument(),

                        PeriodStart = sale.Begin,
                        PeriodEnd = sale.End,
                        OrderId = sale.PartnerOrderId,
                    };

            return messages;
        }
    }
}
