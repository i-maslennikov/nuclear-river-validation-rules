using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, у которых в позиции заказа (простой или пакетной) не указаны объект привязки, должна выводиться ошибка
    /// Проверка пропускается для пакетных позиций у которых IsCompositionOptional = true
    /// "В позиции {0} необходимо указать хотя бы один объект привязки для подпозиции '{1}'" - в случае простой позиции {0} и {1} совпадают
    /// 
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/OrderCheckCompositePositionMustHaveLinkingObject
    ///
    /// * Ошибки по проверкам OrderPositionAdvertisementMustBeCreated и OrderPositionAdvertisementMustHaveAdvertisement
    /// визуально для пользователя не различаются. Можно объединить и оставить универсальное сообщение.
    /// </summary>
    public sealed class OrderPositionAdvertisementMustBeCreated : ValidationResultAccessorBase
    {
       public OrderPositionAdvertisementMustBeCreated(IQuery query) : base(query, MessageTypeCode.OrderPositionAdvertisementMustBeCreated)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                join fail in query.For<Order.MissingOrderPositionAdvertisement>() on order.Id equals fail.OrderId
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
