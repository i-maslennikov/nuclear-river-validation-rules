using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, с незаполненными обязательными для заполнения ЭРМ, дожна выводиться ошибка
    /// "В рекламном материале {0} не заполнен обязательный элемент {1}"
    /// 
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/OrdersCheckPositionMustHaveAdvertisementElements
    /// </summary>
    public sealed class OrderMustHaveAdvertisement : ValidationResultAccessorBase
    {
        public OrderMustHaveAdvertisement(IQuery query) : base(query, MessageTypeCode.OrderMustHaveAdvertisement)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from advertisementId in query.For<Order.OrderPositionAdvertisement>().Where(x => x.OrderId == order.Id).Select(x => x.AdvertisementId).Distinct()
                from advertisement in query.For<Advertisement>().Where(x => x.Id == advertisementId)
                join fail in query.For<Advertisement.RequiredElementMissing>() on advertisement.Id equals fail.AdvertisementId
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
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
