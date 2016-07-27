using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// ƒл€ заказов, позици€ которых ссылаетс€ на не действующий на момент начала размещени€ прайс-лист должна выводитьс€ ошибка.
    /// "ѕозици€ {0} не соответствует актуальному прайс-листу. Ќеобходимо указать позицию из текущего действующего прайс-листа"
    /// 
    /// Source: OrderPositionsRefereceCurrentPriceListOrderValidationRule/OrderCheckOrderPositionDoesntCorrespontToActualPrice
    /// </summary>
    public sealed class OrderPositionShouldCorrespontToActualPriceActor : IActor
    {
        public const int MessageTypeId = 5;

        private static readonly int RuleResultError =
            new ResultBuilder()
                .WhenSingle(Result.Error)
                .WhenMass(Result.Error)
                .WhenMassPrerelease(Result.Error)
                .WhenMassRelease(Result.Error);

        private static readonly int RuleResultWarning =
            new ResultBuilder()
                .WhenSingle(Result.Warning)
                .WhenMass(Result.Warning)
                .WhenMassPrerelease(Result.Warning)
                .WhenMassRelease(Result.Warning);

        private readonly ValidationRuleShared _validationRuleShared;

        public OrderPositionShouldCorrespontToActualPriceActor(ValidationRuleShared validationRuleShared)
        {
            _validationRuleShared = validationRuleShared;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            return _validationRuleShared.ProcessRule(GetValidationResults, MessageTypeId);
        }

        private static IQueryable<Version.ValidationResult> GetValidationResults(IQuery query, long version)
        {
            // ƒействующий прайс заказа - это прайс, действующий на момент начала размещени€ заказа.
            // Ќо есть нюанс: можно одобрить заказ, а потом действующий прайс изменитс€. Ёта ситуаци€ должна вызывать не ошибку, а предупреждение.
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
                        MessageType = MessageTypeId,
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
                        VersionId = version,

                        Result = position.Scope == 0 ? RuleResultWarning : RuleResultError,
                    };

            return messages;
        }
    }
}