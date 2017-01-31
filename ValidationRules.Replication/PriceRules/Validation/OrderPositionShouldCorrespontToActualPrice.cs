using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Messages;
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
    // todo: другой вариант: единичная выдаёт ошибку, массовая - предупреждение, это будет соответствовать поведению erm (учитывая, что единичная для одобренного заказа не вызывается).
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
            var orderPeriodDtos =
                from orderPeriod in query.For<Period.OrderPeriod>()
                from period in query.For<Period>().Where(x => x.Start == orderPeriod.Start && x.OrganizationUnitId == orderPeriod.OrganizationUnitId)
                select new { orderPeriod.OrderId, period.Start, period.End }
                into dto
                group dto by dto.OrderId
                into dtoGroup
                select new
                {
                    OrderId = dtoGroup.Key,
                    Start = dtoGroup.Min(x => x.Start),
                    End = dtoGroup.Max(x => x.End)
                };

            // Действующий прайс заказа - это прайс, действующий на момент начала размещения заказа.
            // Но есть нюанс: можно одобрить заказ, а потом действующий прайс изменится. Эта ситуация должна вызывать не ошибку, а предупреждение.
            var orderPrices =
                from orderPeriodDto in orderPeriodDtos
                from order in query.For<Order>().Where(x => x.Id == orderPeriodDto.OrderId)
                from orderPeriod in query.For<Period.OrderPeriod>().Where(x => x.OrderId == order.Id && x.Start == orderPeriodDto.Start)
                from pricePeriod in query.For<Period.PricePeriod>().Where(x => x.Start == orderPeriod.Start && x.OrganizationUnitId == orderPeriod.OrganizationUnitId)
                from period in query.For<Period>().Where(x => x.Start == orderPeriod.Start && x.OrganizationUnitId == orderPeriod.OrganizationUnitId)
                select new { Order = order, ActualPriceId = pricePeriod.PriceId, Start = period.Start, End = orderPeriodDto.End, ProjectId = period.ProjectId, Scope = orderPeriod.Scope };

            var notRelevantPositions =
                from position in query.For<Order.OrderPricePosition>()
                from actual in orderPrices.Where(x => x.Order.Id == position.OrderId && x.ActualPriceId != position.PriceId)
                select new { Order = actual.Order, Position = position, Start = actual.Start, End = actual.End, ProjectId = actual.ProjectId, Scope = actual.Scope };

            var messages =
                from position in notRelevantPositions
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(new XElement("root",
                            new XElement("order",
                                new XAttribute("id", position.Order.Id)),
                            new XElement("orderPosition",
                                new XAttribute("id", position.Position.OrderPositionId),
                                new XElement("position", new XAttribute("id", position.Position.PositionId)))
                        )),

                        PeriodStart = position.Start,
                        PeriodEnd = position.End,
                        OrderId = position.Order.Id,

                        Result = position.Scope == 0 ? RuleResultWarning : RuleResultError,
                    };

            return messages;
        }
    }
}