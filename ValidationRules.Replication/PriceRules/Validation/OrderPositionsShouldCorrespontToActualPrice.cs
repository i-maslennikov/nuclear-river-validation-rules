using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// ƒл€ заказов, в городах без действующего на момент размещени€ прайс-листа, должна выводитьс€ ошибка.
    /// "«аказ не соответствуют актуальному прайс-листу. Ќеобходимо указать позиции из текущего действующего прайс-листа"
    /// 
    /// Source: OrderPositionsRefereceCurrentPriceListOrderValidationRule/OrderCheckOrderPositionsDoesntCorrespontToActualPrice
    /// TODO: странный текст ошибки, нужно исправить.
    /// </summary>
    public class OrderPositionsShouldCorrespontToActualPrice : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                            .WhenMass(Result.Error)
                                                            .WhenMassPrerelease(Result.Error)
                                                            .WhenMassRelease(Result.Error);

        public OrderPositionsShouldCorrespontToActualPrice(IQuery query) : base(query, 3)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            // проверка провер€ет соответствие только первого периода
            var orderFirstPeriods = from orderPeriod1 in query.For<OrderPeriod>()
                                    from orderPeriod2 in query.For<OrderPeriod>().Where(x => orderPeriod1.OrderId == x.OrderId && orderPeriod1.Start > x.Start).DefaultIfEmpty()
                                    where orderPeriod2 == null
                                    select orderPeriod1;

            var orderFirstPeriodDtos = from orderFirstPeriod in orderFirstPeriods
                                  join order in query.For<Order>() on orderFirstPeriod.OrderId equals order.Id
                                  join period in query.For<Period>()
                                  on new { orderFirstPeriod.OrganizationUnitId, orderFirstPeriod.Start } equals new { period.OrganizationUnitId, period.Start }
                                  select new
                                  {
                                      OrderId = order.Id,
                                      Number = order.Number,

                                      period.ProjectId,
                                      period.Start,
                                      period.End,
                                  };

            var priceNotFoundErrors =
                from orderFirstPeriodDto in orderFirstPeriodDtos
                from pricePeriod in query.For<PricePeriod>().Where(x => x.OrganizationUnitId == orderFirstPeriodDto.ProjectId && x.Start == orderFirstPeriodDto.Start).DefaultIfEmpty()
                where pricePeriod == null
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(new XElement("root",
                                                                   new XElement("order",
                                                                                new XAttribute("id", orderFirstPeriodDto.OrderId),
                                                                                new XAttribute("number", orderFirstPeriodDto.Number)))),
                        PeriodStart = orderFirstPeriodDto.Start,
                        PeriodEnd = orderFirstPeriodDto.End,
                        ProjectId = orderFirstPeriodDto.ProjectId,

                        Result = RuleResult,
                    };

            return priceNotFoundErrors;
        }
    }
}