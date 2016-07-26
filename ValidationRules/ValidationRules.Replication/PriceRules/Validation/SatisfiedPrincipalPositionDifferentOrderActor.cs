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
    /// Для заказов в статусе "на расторжении", если они содержат основную позицию для остающихся одобренными заказов и среди одобренных нет другой основной позиции должно выводиться предупреждение.
    /// Позиция {0} данного Заказа является основной для следующих позиций:
    /// Позиция {0} Заказа {1}
    /// 
    /// Source: ADP/ADPValidation_Template
    /// 
    /// Q1: X является сопутствующей для Y (объект привязки совпадает)
    ///     Оформлен заказ №1 с X(A,B), Y(A) и заказ №2 с Y(B).
    ///     Заказ №2 отправляется на расторжение. Должна ли быть ошибка?
    /// A: Да.
    /// 
    /// Q2: Аналогично Q1, но позиция X ещё является сопутствующей для Z (без учёта)
    ///     И позиция Z продана в заказ 1. По логике (бага?) проверки LinkedObjectsMissedInPrincipalsActor (Q3) ошибка должна быть.
    /// A: Ошибка есть.
    /// </summary>
    public sealed class SatisfiedPrincipalPositionDifferentOrderActor : IActor
    {
        public const int MessageTypeId = 15;

        private const int NoDependency = 2;
        private const int Match = 1;
        private const int Different = 3;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Warning)
                                                                    .WhenMassPrerelease(Result.Warning)
                                                                    .WhenMassRelease(Result.Warning);

        private readonly ValidationRuleShared _validationRuleShared;

        public SatisfiedPrincipalPositionDifferentOrderActor(ValidationRuleShared validationRuleShared)
        {
            _validationRuleShared = validationRuleShared;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            return _validationRuleShared.ProcessRule(GetValidationResults, MessageTypeId);
        }

        private static IQueryable<Version.ValidationResult> GetValidationResults(IQuery query, long version)
        {
            var orderPositions =
                from order in query.For<Order>()
                join period in query.For<OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<OrderPosition>() on order.Id equals position.OrderId
                select new { order.FirmId, period.Start, period.OrganizationUnitId, period.Scope, position };

            var associatedPositions =
                from order in query.For<Order>()
                join period in query.For<OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<OrderAssociatedPosition>() on order.Id equals position.OrderId
                select new { order.FirmId, period.Start, period.OrganizationUnitId, period.Scope, position };

            var satisfiedPositions =
                from orderPosition in orderPositions
                join associatedPosition in associatedPositions on
                    new { orderPosition.FirmId, orderPosition.Start, orderPosition.OrganizationUnitId, orderPosition.position.ItemPositionId } equals
                    new { associatedPosition.FirmId, associatedPosition.Start, associatedPosition.OrganizationUnitId, ItemPositionId = associatedPosition.position.PrincipalPositionId }
                where
                    // правила удовлетворённые из других заказов (сопутствующая в одном заказе, основная - в другом)
                    orderPosition.position.OrderId != associatedPosition.position.OrderId &&
                    orderPosition.position.OrderPositionId != associatedPosition.position.CauseOrderPositionId
                where
                    // local scope only
                    orderPosition.Scope != 0 || orderPosition.Scope == associatedPosition.Scope
                where
                    associatedPosition.position.BindingType == NoDependency ||

                    (associatedPosition.position.BindingType == Match &&
                    associatedPosition.position.HasNoBinding == orderPosition.position.HasNoBinding &&
                    (associatedPosition.position.Category3Id == null || associatedPosition.position.Category3Id == orderPosition.position.Category3Id) &&
                    (associatedPosition.position.Category1Id == null || associatedPosition.position.Category1Id == orderPosition.position.Category1Id) &&
                    (associatedPosition.position.FirmAddressId == null || associatedPosition.position.FirmAddressId == orderPosition.position.FirmAddressId)) ||

                    (associatedPosition.position.BindingType == Different &&
                    (associatedPosition.position.HasNoBinding != orderPosition.position.HasNoBinding ||
                    (associatedPosition.position.Category3Id != null && associatedPosition.position.Category3Id != orderPosition.position.Category3Id) ||
                    (associatedPosition.position.Category1Id != null && associatedPosition.position.Category1Id != orderPosition.position.Category1Id) ||
                    (associatedPosition.position.FirmAddressId != null && associatedPosition.position.FirmAddressId != orderPosition.position.FirmAddressId)))
                select new
                {
                    associatedPosition.FirmId,
                    CauseOrderId = associatedPosition.position.OrderId,
                    associatedPosition.position.CauseOrderPositionId,
                    associatedPosition.position.CausePackagePositionId,
                    associatedPosition.position.CauseItemPositionId,

                    associatedPosition.position.PrincipalPositionId,
                    PrincipalOrderPositionId = orderPosition.position.OrderPositionId,
                    PrincipalOrderId = orderPosition.position.OrderId,

                    associatedPosition.Start,
                    associatedPosition.OrganizationUnitId,
                };

            var messages = from warning in satisfiedPositions
                           join period in query.For<Period>() on new { warning.Start, warning.OrganizationUnitId } equals new { period.Start, period.OrganizationUnitId }
                           select new Version.ValidationResult
                               {
                                   VersionId = version,
                                   MessageType = MessageTypeId,
                                   MessageParams =
                                       new XDocument(new XElement("root",
                                                                  new XElement("firm",
                                                                               new XAttribute("id", warning.FirmId)),
                                                                  new XElement("position",
                                                                               new XAttribute("orderId", warning.PrincipalOrderId),
                                                                               new XAttribute("orderNumber", query.For<Order>().Single(x => x.Id == warning.PrincipalOrderId).Number),
                                                                               new XAttribute("orderPositionId", warning.PrincipalOrderPositionId),
                                                                               new XAttribute("orderPositionName", query.For<Position>().Single(x => x.Id == warning.PrincipalOrderPositionId).Name)),
                                                                  new XElement("position",
                                                                               new XAttribute("orderId", warning.CauseOrderId),
                                                                               new XAttribute("orderNumber", query.For<Order>().Single(x => x.Id == warning.CauseOrderId).Number),
                                                                               new XAttribute("orderPositionId", warning.CauseOrderPositionId),
                                                                               new XAttribute("orderPositionName", query.For<Position>().Single(x => x.Id == warning.CausePackagePositionId).Name)),
                                                                  new XElement("order",
                                                                               new XAttribute("id", warning.PrincipalOrderId),
                                                                               new XAttribute("number", query.For<Order>().Single(x => x.Id == warning.PrincipalOrderId).Number)))),

                                   PeriodStart = period.Start,
                                   PeriodEnd = period.End,
                                   ProjectId = period.ProjectId,

                                   Result = RuleResult,
                               };

            return messages;
        }
    }
}
