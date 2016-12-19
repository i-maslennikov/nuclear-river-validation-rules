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
            var orders =
                from order in query.For<Order>()
                from start in query.For<OrderPeriod>().Where(x => x.OrderId == order.Id)
                from end in query.For<OrderPeriod>().Where(x => x.OrderId == order.Id).SelectMany(x => query.For<Period>().Where(y => y.Start == x.Start && y.OrganizationUnitId == x.OrganizationUnitId))
                group new { start.Start, end.End } by new { order.Id, order.Number, start.OrganizationUnitId } into groups
                select new { groups.Key.Id, groups.Key.Number, groups.Key.OrganizationUnitId, Start = groups.Min(x => x.Start), End = groups.Max(x => x.End) };

            var result =
                from order in orders
                where !query.For<PricePeriod>().Any(x => x.Start <= order.Start && x.OrganizationUnitId == order.OrganizationUnitId)
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(new XElement("root",
                            new XElement("order",
                                new XAttribute("id", order.Id),
                                new XAttribute("name", order.Number)))),

                        PeriodStart = order.Start,
                        PeriodEnd = order.End,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return result;
        }
    }
}