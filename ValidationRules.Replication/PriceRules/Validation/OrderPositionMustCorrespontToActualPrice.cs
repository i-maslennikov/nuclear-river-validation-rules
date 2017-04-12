using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказов, позиция которых ссылается на не действующий на момент начала размещения прайс-лист должна выводиться ошибка.
    /// "Позиция {0} не соответствует актуальному прайс-листу. Необходимо указать позицию из текущего действующего прайс-листа"
    /// 
    /// Source: OrderPositionsRefereceCurrentPriceListOrderValidationRule/OrderCheckOrderPositionDoesntCorrespontToActualPrice
    /// 
    /// Есть нюанс: можно одобрить заказ, а потом действующий прайс изменится. Эта ситуация должна вызывать не ошибку, а предупреждение.
    ///             Для заказов в статусе "На расторжении" эта проверка не должна мешать их вернуть в размещение
    /// </summary>
    public sealed class OrderPositionMustCorrespontToActualPrice : ValidationResultAccessorBase
    {
        public OrderPositionMustCorrespontToActualPrice(IQuery query) : base(query, MessageTypeCode.OrderPositionMustCorrespontToActualPrice)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var messages =
                from order in query.For<Order>().Where(x => !x.IsCommitted)
                from actualPrice in query.For<Order.ActualPrice>()
                                         .Where(x => x.PriceId.HasValue)
                                         .Where(x => x.OrderId == order.Id)
                from orderPosition in query.For<Order.OrderPricePosition>()
                                           .Where(x => x.OrderId == order.Id && x.PriceId != actualPrice.PriceId.Value)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(orderPosition.OrderPositionId,
                                        new Reference<EntityTypeOrder>(order.Id),
                                        new Reference<EntityTypePosition>(orderPosition.PositionId)))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistribution,
                        PeriodEnd = order.EndDistributionPlan,
                        OrderId = order.Id,
                    };

            return messages;
        }
    }
}