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
    /// Есть нюанс: можно одобрить заказ, а потом действующий прайс изменится. Эта ситуация должна вызывать не ошибку, а предупреждение.
    /// </summary>
    public sealed class OrderPositionMustCorrespontToActualPrice : ValidationResultAccessorBase
    {
        public OrderPositionMustCorrespontToActualPrice(IQuery query) : base(query, MessageTypeCode.OrderPositionMustCorrespontToActualPrice)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var orders =
                from orderPeriod in query.For<Period.OrderPeriod>()
                from period in query.For<Period>().Where(x => x.Start == orderPeriod.Start && x.OrganizationUnitId == orderPeriod.OrganizationUnitId)
                select new { orderPeriod.OrderId, period.Start, period.End }
                into dto
                group dto by dto.OrderId
                into dtoGroup
                select new
                {
                    Id = dtoGroup.Key,
                    Start = dtoGroup.Min(x => x.Start),
                    End = dtoGroup.Max(x => x.End)
                };

            var messages =
                from order in orders
                from orderPosition in query.For<Order.OrderPricePosition>()
                                           .Where(x => x.OrderId == order.Id)
                from actualPrice in query.For<Order.ActualPrice>()
                                         .Where(x => x.PriceId != null)
                                         .Where(x => x.OrderId == order.Id)
                from orderPeriod in query.For<Period.OrderPeriod>()
                                         .Where(x => x.OrderId == order.Id && x.Start == order.Start)
                where orderPosition.PriceId != actualPrice.PriceId.Value // прайс-лист позиции заказа отличается от актуального прайс-листа заказа
                where orderPeriod.Scope != 0                             // заказ не в статусе Одобрен
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(orderPosition.OrderPositionId,
                                        new Reference<EntityTypeOrder>(order.Id),
                                        new Reference<EntityTypePosition>(orderPosition.PositionId)))
                                .ToXDocument(),

                        PeriodStart = order.Start,
                        PeriodEnd = order.End,
                        OrderId = order.Id,
                    };

            return messages;
        }
    }
}