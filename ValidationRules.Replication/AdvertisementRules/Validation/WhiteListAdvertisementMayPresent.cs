using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для фирм, у которых выбран в белый список РМ, должно выводиться информационное сообщение
    /// Для фирмы {0} в белый список выбран рекламный материал {1}
    /// 
    /// Source: AdvertisementsOnlyWhiteListOrderValidationRule/AdvertisementChoosenForWhitelist
    /// 
    /// * Поскольку проверок фирм нет, то сообщения выводим в заказах этой фирмы, в которых есть как минимум один РМ с возможностью выбора в белый список.
    /// </summary>
    public sealed class WhiteListAdvertisementMayPresent : ValidationResultAccessorBase
    {
        public WhiteListAdvertisementMayPresent(IQuery query) : base(query, MessageTypeCode.WhiteListAdvertisementMayPresent)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ordersProvideWhiteListAdvertisement =
                from order in query.For<Order>().Where(x => x.RequireWhiteListAdvertisement)
                where order.ProvideWhiteListAdvertisementId.HasValue
                select new
                {
                    FirmId = order.FirmId,
                    AdvertisementId = order.ProvideWhiteListAdvertisementId.Value,

                    PeriodStart = order.BeginDistributionDate,
                    PeriodEnd = order.EndDistributionDatePlan,
                    OrderId = order.Id,
                };

            var ordersNotProvideWhiteListAdvertisement =
                from order in query.For<Order>().Where(x => x.RequireWhiteListAdvertisement)
                from period in query.For<Firm.WhiteListDistributionPeriod>()
                                    .Where(x => x.FirmId == order.FirmId && x.Start < order.EndDistributionDatePlan && order.BeginDistributionDate < x.End)
                                    .Where(x => x.ProvidedByOrderId.HasValue)
                where !order.ProvideWhiteListAdvertisementId.HasValue
                select new
                {
                    FirmId = order.FirmId,
                    AdvertisementId = period.AdvertisementId.Value,

                    PeriodStart = period.Start > order.BeginDistributionDate ? period.Start : order.BeginDistributionDate,
                    PeriodEnd = period.End < order.EndDistributionDatePlan ? period.End : order.EndDistributionDatePlan,
                    OrderId = order.Id,
                };

            var result =
                from order in ordersProvideWhiteListAdvertisement.Concat(ordersNotProvideWhiteListAdvertisement)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeFirm>(order.FirmId),
                                    new Reference<EntityTypeAdvertisement>(order.AdvertisementId))
                                .ToXDocument(),

                        PeriodStart = order.PeriodStart,
                        PeriodEnd = order.PeriodEnd,
                        OrderId = order.OrderId,
                    };

            return result;
        }
    }
}
