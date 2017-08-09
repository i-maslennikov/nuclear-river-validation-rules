using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, у которых хотя бы один шаблон РМ обязательный, и для этого шаблона не указан РМ, должна выводиться ошибка:
    /// "В позиции {0} необходимо указать рекламные материалы"
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/OrderCheckPositionMustHaveAdvertisements
    /// 
    /// "В позиции {0} необходимо указать рекламные материалы для подпозиции '{1}'"
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/OrderCheckCompositePositionMustHaveAdvertisements
    /// </summary>
    public sealed class OrderPositionAdvertisementMustHaveAdvertisement : ValidationResultAccessorBase
    {
        public OrderPositionAdvertisementMustHaveAdvertisement(IQuery query) : base(query, MessageTypeCode.OrderPositionAdvertisementMustHaveAdvertisement)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                join fail in query.For<Order.MissingAdvertisementReference>() on order.Id equals fail.OrderId
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(fail.OrderPositionId,
                                        new Reference<EntityTypeOrder>(order.Id),
                                        new Reference<EntityTypePosition>(fail.CompositePositionId)),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(fail.OrderPositionId),
                                        new Reference<EntityTypePosition>(fail.PositionId)))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistributionDate,
                        PeriodEnd = order.EndDistributionDatePlan,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
