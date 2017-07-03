using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для фирм, у которых недостаёт выбранных в белый список РМ, должна выводиться ошибка при массовой проверке и предупреждение при единичной.
    /// "Для фирмы {0} не указан рекламный материал в белый список"
    /// 
    /// Source: AdvertisementsOnlyWhiteListOrderValidationRule/AdvertisementForWhitelistDoesNotSpecified
    /// 
    /// * Поскольку проверок фирм нет, то сообщения выводим в заказах этой фирмы, в которых есть как минимум один РМ с возможностью выбора в белый список.
    /// * "Недостаёт" - значит, в выпуск выходит как минимум один РМ с возможностью выбора в белый список, но ни одного выбранного.
    /// 
    /// Q: Если РМ в белый список есть в заказе "на утверждении", должна ли данная ошибка появляться в одобренном заказе?
    /// A: Erm сейчас в единичной проверке одобренного заказа выдаёт сообщение, что РМ в белый список есть, а при массовой - выдаёт сообщение, что нету.
    ///    Мы будем выдавать ошибку для любых (требующих) заказов по фирме, пока в одобренных не появится РМ в белый список.
    /// </summary>
    public sealed class WhiteListAdvertisementMustPresent : ValidationResultAccessorBase
    {
        public WhiteListAdvertisementMustPresent(IQuery query) : base(query, MessageTypeCode.WhiteListAdvertisementMustPresent)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>().Where(x => x.RequireWhiteListAdvertisement)
                from period in query.For<Firm.WhiteListDistributionPeriod>()
                                             .Where(x => x.FirmId == order.FirmId && x.Start < order.EndDistributionDatePlan && order.BeginDistributionDate < x.End)
                                             .Where(x => x.ProvidedByOrderId == null)
                where !order.ProvideWhiteListAdvertisementId.HasValue
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeFirm>(order.FirmId))
                                .ToXDocument(),

                        PeriodStart = period.Start > order.BeginDistributionDate ? period.Start : order.BeginDistributionDate,
                        PeriodEnd = period.End < order.EndDistributionDatePlan ? period.End : order.EndDistributionDatePlan,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
