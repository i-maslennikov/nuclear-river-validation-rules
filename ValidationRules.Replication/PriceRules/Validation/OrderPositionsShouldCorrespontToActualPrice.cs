using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

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
            var result =
                from order in query.For<Order>()
                from begin in query.For<OrderPeriod>().OrderBy(x => x.Start).Where(x => x.OrderId == order.Id).Take(1)
                from end in query.For<OrderPeriod>().OrderByDescending(x => x.Start).Where(x => x.OrderId == order.Id).Take(1)
                from period in query.For<Period>().Where(x => x.Start == end.Start && x.OrganizationUnitId == end.OrganizationUnitId)
                let price = query.For<PricePeriod>().OrderBy(x => x.Start).FirstOrDefault(x => x.Start <= begin.Start && x.OrganizationUnitId == begin.OrganizationUnitId)
                where price == null
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(new XElement("root",
                            new XElement("order",
                                new XAttribute("id", order.Id),
                                new XAttribute("name", order.Number)))),

                        PeriodStart = begin.Start,
                        PeriodEnd = period.End,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return result;
        }
    }
}