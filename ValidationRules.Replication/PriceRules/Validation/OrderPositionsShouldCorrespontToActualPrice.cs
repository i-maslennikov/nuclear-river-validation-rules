using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказов, в городах без действующего на момент размещения прайс-листа, должна выводиться ошибка.
    /// "Заказ не соответствуют актуальному прайс-листу. Необходимо указать позиции из текущего действующего прайс-листа"
    /// 
    /// Source: OrderPositionsRefereceCurrentPriceListOrderValidationRule/OrderCheckOrderPositionsDoesntCorrespontToActualPrice
    /// TODO: странный текст ошибки, нужно исправить.
    /// </summary>
    public class OrderPositionsShouldCorrespontToActualPrice : ValidationResultAccessorBase
    {
        public OrderPositionsShouldCorrespontToActualPrice(IQuery query) : base(query, MessageTypeCode.OrderPositionsShouldCorrespontToActualPrice)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var messages =
                from order in query.For<Order>()
                from actualPrice in query.For<Order.ActualPrice>().Where(x => x.PriceId == null).Where(x => x.OrderId == order.Id)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrder>(order.Id))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistribution,
                        PeriodEnd = order.EndDistributionPlan,
                        OrderId = order.Id,
                    };

            return messages;
        }
    }
}