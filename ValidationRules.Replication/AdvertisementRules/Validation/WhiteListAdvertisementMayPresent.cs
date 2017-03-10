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
            var ruleResults =
                from order in query.For<Order>().Where(x => x.RequireWhiteListAdvertisement)
                from advertisement in query.For<Advertisement>().Where(x => x.FirmId == order.FirmId && x.IsSelectedToWhiteList)
                from period in query.For<Firm.WhiteListDistributionPeriod>()
                                    .Where(x => x.FirmId == order.FirmId && x.Start < order.EndDistributionDatePlan && order.BeginDistributionDate < x.End)
                                    .Where(x => x.ProvidedByOrderId.HasValue).DefaultIfEmpty()
                where period != null || order.ProvideWhiteListAdvertisement
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeFirm>(order.FirmId),
                                    new Reference<EntityTypeAdvertisement>(advertisement.Id))
                                .ToXDocument(),

                        PeriodStart = period != null && period.Start > order.BeginDistributionDate ? period.Start : order.BeginDistributionDate,
                        PeriodEnd = period != null && period.End < order.EndDistributionDatePlan ? period.End : order.EndDistributionDatePlan,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
