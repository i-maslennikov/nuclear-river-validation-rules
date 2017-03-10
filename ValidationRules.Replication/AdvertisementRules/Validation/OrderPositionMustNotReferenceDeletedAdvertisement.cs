using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказаов, у которых есть удалённые РМ, должна выводиться ошибка
    /// В позиции {0} выбран удалённый рекламный материал {1}
    /// 
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/RemovedAdvertisemendSpecifiedForPosition
    /// </summary>
    public sealed class OrderPositionMustNotReferenceDeletedAdvertisement : ValidationResultAccessorBase
    {
        public OrderPositionMustNotReferenceDeletedAdvertisement(IQuery query) : base(query, MessageTypeCode.OrderPositionMustNotReferenceDeletedAdvertisement)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                join fail in query.For<Order.AdvertisementDeleted>() on order.Id equals fail.OrderId
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrder>(order.Id),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(fail.OrderPositionId),
                                        new Reference<EntityTypePosition>(fail.PositionId)),
                                    new Reference<EntityTypeAdvertisement>(fail.AdvertisementId))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistributionDate,
                        PeriodEnd = order.EndDistributionDatePlan,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
