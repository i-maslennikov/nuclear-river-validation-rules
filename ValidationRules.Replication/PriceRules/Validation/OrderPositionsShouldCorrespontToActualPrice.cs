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
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                            .WhenMass(Result.None)
                                                            .WhenMassPrerelease(Result.None)
                                                            .WhenMassRelease(Result.None);

        public OrderPositionsShouldCorrespontToActualPrice(IQuery query) : base(query, MessageTypeCode.OrderPositionsShouldCorrespontToActualPrice)
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
                from actualPrice in query.For<Order.ActualPrice>().Where(x => x.OrderId == order.Id)
                where actualPrice.PriceId == null // не нашли актуальный прайс-лист
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrder>(order.Id))
                                .ToXDocument(),

                        PeriodStart = order.Start,
                        PeriodEnd = order.End,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return messages;
        }
    }
}