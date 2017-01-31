using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказов, позиции которых ссылаются на не действительные номенклатурные позиции, должна выводиться ошибка.
    /// "Позиция {0} соответствует скрытой позиции прайс листа. Необходимо указать активную позицию из текущего действующего прайс-листа"
    /// 
    /// Source: OrderPositionsRefereceCurrentPriceListOrderValidationRule/OrderCheckOrderPositionCorrespontToInactivePosition
    /// 
    /// Q: Получается, если позицию скрыли после одобрения - то заказ попадёт в сборку?
    /// 
    /// todo: Убрать из этой группы проверок - понятия периода/scope не имеют смысла для этой проверки
    /// </summary>
    public sealed class OrderPositionCorrespontToInactivePosition : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.None)
                                                                    .WhenMassRelease(Result.None);

        public OrderPositionCorrespontToInactivePosition(IQuery query) : base(query, MessageTypeCode.OrderPositionCorrespontToInactivePosition)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            // проверка проверяет соответствие только первого периода
            var orderFirstPeriods = from orderPeriod1 in query.For<Period.OrderPeriod>()
                                    from orderPeriod2 in query.For<Period.OrderPeriod>().Where(x => orderPeriod1.OrderId == x.OrderId && orderPeriod1.Start > x.Start).DefaultIfEmpty()
                                    where orderPeriod2 == null
                                    select orderPeriod1;

            var orderFirstPeriodDtos = from orderFirstPeriod in orderFirstPeriods
                                  join order in query.For<Order>() on orderFirstPeriod.OrderId equals order.Id
                                  join period in query.For<Period>()
                                  on new { orderFirstPeriod.OrganizationUnitId, orderFirstPeriod.Start } equals new { period.OrganizationUnitId, period.Start }
                                  select new
                                  {
                                      OrderId = order.Id,

                                      period.ProjectId,
                                      period.Start,
                                      period.End,
                                  };

            var pricePositionIsNotActiveErrors =
                from orderFirstPeriodDto in orderFirstPeriodDtos
                join orderPricePosition in query.For<Order.OrderPricePosition>() on orderFirstPeriodDto.OrderId equals orderPricePosition.OrderId
                where !orderPricePosition.IsActive
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(new XElement("root",
                            new XElement("order",
                                new XAttribute("id", orderFirstPeriodDto.OrderId)),
                            new XElement("orderPosition",
                                new XAttribute("id", orderPricePosition.OrderPositionId),
                                new XElement("position", new XAttribute("id", orderPricePosition.PositionId)))
                        )),

                        PeriodStart = orderFirstPeriodDto.Start,
                        PeriodEnd = orderFirstPeriodDto.End,
                        OrderId = orderFirstPeriodDto.OrderId,

                        Result = RuleResult,
                    };

            return pricePositionIsNotActiveErrors;
        }
    }
}