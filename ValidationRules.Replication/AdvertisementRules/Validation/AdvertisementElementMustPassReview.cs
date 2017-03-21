using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, в которых есть невыверенные РМ, должна выводиться ошибка
    /// - В рекламном материале "{0}", который подлежит выверке, элемент "{1}" находится в статусе 'Черновик'
    /// 
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/OrdersCheckAdvertisementElementIsDraft
    /// 
    /// - В рекламном материале "{0}", который подлежит выверке, элемент "{1}" содержит ошибки выверки
    /// 
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/OrdersCheckAdvertisementElementWasInvalidated
    /// </summary>
    public sealed class AdvertisementElementMustPassReview : ValidationResultAccessorBase
    {
        public AdvertisementElementMustPassReview(IQuery query) : base(query, MessageTypeCode.AdvertisementElementMustPassReview)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from advertisementId in query.For<Order.OrderPositionAdvertisement>().Where(x => x.OrderId == order.Id).Select(x => x.AdvertisementId).Distinct()
                from advertisement in query.For<Advertisement>().Where(x => x.Id == advertisementId)
                from fail in query.For<Advertisement.ElementNotPassedReview>().Where(x => x.AdvertisementId == advertisement.Id)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "advertisementElementStatus", (int)fail.Status } },
                                    new Reference<EntityTypeOrder>(order.Id),
                                    new Reference<EntityTypeAdvertisement>(advertisement.Id),
                                    new Reference<EntityTypeAdvertisementElement>(fail.AdvertisementElementId,
                                        new Reference<EntityTypeAdvertisementElementTemplate>(fail.AdvertisementElementTemplateId)))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistributionDate,
                        PeriodEnd = order.EndDistributionDatePlan,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
