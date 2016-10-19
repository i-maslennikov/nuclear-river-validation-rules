using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказов, позиция которых ссылается на не действующий на момент начала размещения прайс-лист должна выводиться ошибка.
    /// "Позиция {0} не соответствует актуальному прайс-листу. Необходимо указать позицию из текущего действующего прайс-листа"
    /// 
    /// Source: OrderPositionsRefereceCurrentPriceListOrderValidationRule/OrderCheckOrderPositionDoesntCorrespontToActualPrice
    /// </summary>
    // todo: подумать о рефакторинге: актуальный прайс-лист должен вычисляться на этапе агрегатов, а эту проверку разделить на две.
    public sealed class OrderPositionShouldCorrespontToActualPrice : ValidationResultAccessorBase
    {
        private static readonly int RuleResultError =
            new ResultBuilder()
                .WhenSingle(Result.Error)
                .WhenMass(Result.None)
                .WhenMassPrerelease(Result.None)
                .WhenMassRelease(Result.None);

        private static readonly int RuleResultWarning =
            new ResultBuilder()
                .WhenSingle(Result.Warning)
                .WhenMass(Result.None)
                .WhenMassPrerelease(Result.None)
                .WhenMassRelease(Result.None);

        public OrderPositionShouldCorrespontToActualPrice(IQuery query) : base(query, MessageTypeCode.OrderPositionShouldCorrespontToActualPrice)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            // Действующий прайс заказа - это прайс, действующий на момент начала размещения заказа.
            // Но есть нюанс: можно одобрить заказ, а потом действующий прайс изменится. Эта ситуация должна вызывать не ошибку, а предупреждение.
            var orderPrices =
                from order in query.For<Order>()
                let orderStart = query.For<OrderPeriod>().Where(x => x.OrderId == order.Id).Min(x => x.Start)
                let orderEnd = query.For<OrderPeriod>().SelectMany(x => query.For<Period>().Where(y => x.OrderId == order.Id && y.Start == x.Start && y.OrganizationUnitId == x.OrganizationUnitId)).Max(x => x.End)
                from orderPeriod in query.For<OrderPeriod>().Where(x => x.OrderId == order.Id && x.Start == orderStart)
                from pricePeriod in query.For<PricePeriod>().Where(x => x.Start == orderPeriod.Start && x.OrganizationUnitId == orderPeriod.OrganizationUnitId)
                from period in query.For<Period>().Where(x => x.Start == orderPeriod.Start && x.OrganizationUnitId == orderPeriod.OrganizationUnitId)
                select new { Order = order, ActualPriceId = pricePeriod.PriceId, Start = period.Start, End = orderEnd, ProjectId = period.ProjectId, Scope = orderPeriod.Scope };

            var notRelevantPositions =
                from position in query.For<OrderPricePosition>()
                from actual in orderPrices.Where(x => x.Order.Id == position.OrderId && x.ActualPriceId != position.PriceId)
                select new { Order = actual.Order, Position = position, Start = actual.Start, End = actual.End, ProjectId = actual.ProjectId, Scope = actual.Scope };

            var messages =
                from position in notRelevantPositions
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(new XElement("root",
                                                                   new XElement("order",
                                                                                new XAttribute("id", position.Order.Id),
                                                                                new XAttribute("number", position.Order.Number)),
                                                                   new XElement("orderPosition",
                                                                                new XAttribute("id", position.Position.OrderPositionId),
                                                                                new XAttribute("name", position.Position.OrderPositionName)))),
                        PeriodStart = position.Start,
                        PeriodEnd = position.End,
                        ProjectId = position.ProjectId,

                        Result = position.Scope == 0 ? RuleResultWarning : RuleResultError,
                    };

            return messages;
        }
    }
}