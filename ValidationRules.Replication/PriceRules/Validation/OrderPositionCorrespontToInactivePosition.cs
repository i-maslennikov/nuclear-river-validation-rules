using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказов, позиции которых ссылаются на недействительные номенклатурные позиции, должна выводиться ошибка.
    /// "Позиция {0} соответствует скрытой позиции прайс листа. Необходимо указать активную позицию из текущего действующего прайс-листа"
    /// 
    /// Source: OrderPositionsRefereceCurrentPriceListOrderValidationRule/OrderCheckOrderPositionCorrespontToInactivePosition
    /// 
    /// Q: Получается, если позицию скрыли после одобрения - то заказ попадёт в сборку?
    /// 
    /// </summary>
    public sealed class OrderPositionCorrespontToInactivePosition : ValidationResultAccessorBase
    {
        public OrderPositionCorrespontToInactivePosition(IQuery query) : base(query, MessageTypeCode.OrderPositionCorrespontToInactivePosition)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var messages =
                from order in query.For<Order>()
                from orderPricePosition in query.For<Order.OrderPricePosition>().Where(x => !x.IsActive).Where(x => x.OrderId == order.Id)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(orderPricePosition.OrderPositionId,
                                        new Reference<EntityTypeOrder>(order.Id),
                                        new Reference<EntityTypePosition>(orderPricePosition.PositionId)))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistribution,
                        PeriodEnd = order.EndDistributionPlan,
                        OrderId = order.Id,
                    };

            return messages;
        }
    }
}