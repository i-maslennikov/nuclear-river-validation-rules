using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.PriceRules.Validation.Dto;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказов, в котором есть сопутствующая позиция и есть основная, но не удовлетворено условие ObjectBindingType.Different, должна выводиться ошибка.
    /// "{0} содержит объекты привязки, конфликтующие с объектами привязки следующей основной позиции: {1}"
    /// 
    /// Source: AssociatedAndDeniedPricePositionsOrderValidationRule/ConflictingPrincipalPosition
    /// 
    /// Q1: Позиция Y - сопутствующая для X (с требованием, чтобы объекты привязки различались).
    ///     Две X проданы в рубрики A и B. Продана Y в рубрику A. Должна ли появиться ошибка?
    /// A: Должна появиться ошибка "Позиция Y содержит объекты привязки, конфликтующие с объектами привязки следующей основной позиции: X"
    /// 
    /// Q2: Позиция Y - сопутствующая для X (с требованием, чтобы объекты привязки различались).
    ///     X продана в рубрику адреса (A, B). Y продана в рубрику B. Должна ли появиться ошибка?
    /// A: Нет ошибки.
    /// 
    /// Q3: Позиция Y - сопутствующая для X (с требованием, чтобы объекты привязки различались).
    ///     X продана в рубрику адреса (A, B). Y продана к адресу A. Должна ли появиться ошибка?
    /// A: Нет ошибки.
    /// 
    /// Q4: Позиция Y - сопутствующая для X1 (с требованием, чтобы объекты привязки различались), X2 (без учёта объектов привязки)
    ///     X1 продана в рубрику (A). X2 продана в рубрику (B). Y продана в рубрику A. Должна ли появиться ошибка?
    /// A: Не отличается от Q1. "Позиция Y содержит объекты привязки, конфликтующие с объектами привязки следующей основной позиции: X"
    /// 
    /// Q5: Позиция Y - сопутствующая для X (с требованием, чтобы объекты привязки различались).
    ///     X продана для A, Y продана для A - заказ одобрен без ошибок.
    ///     X продана для B - можно ли одобрить заказ?
    /// A: Erm позволяет одобрить заказ, но при проверке первого - появляется ошибка.
    /// </summary>
    public sealed class ConflictingPrincipalPosition : ValidationResultAccessorBase
    {
        private const int Different = 3;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public ConflictingPrincipalPosition(IQuery query) : base(query, MessageTypeCode.ConflictingPrincipalPosition)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var orderPositions =
                from order in query.For<Order>()
                join period in query.For<OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<OrderPosition>() on order.Id equals position.OrderId
                select new Dto<OrderPosition> { FirmId = order.FirmId, Start = period.Start, OrganizationUnitId = period.OrganizationUnitId, Scope = period.Scope, Position = position };

            var associatedPositions =
                from order in query.For<Order>()
                join period in query.For<OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<OrderAssociatedPosition>() on order.Id equals position.OrderId
                where position.BindingType == Different
                select new Dto<OrderAssociatedPosition> { FirmId = order.FirmId, Start = period.Start, OrganizationUnitId = period.OrganizationUnitId, Scope = period.Scope, Position = position };

            var conflictingPositions =
                from pair in associatedPositions.SelectMany(Specs.Join.Aggs.WithMatchedBindingObject(orderPositions), (associated, principal) => new { associated, principal })
                join period in query.For<Period>() on new { pair.associated.Start, pair.associated.OrganizationUnitId } equals new { period.Start, period.OrganizationUnitId }
                select new
                    {
                        FirmId = pair.associated.FirmId,
                        Start = period.Start,
                        End = period.End,
                        ProjectId = period.ProjectId,
                        OrderPrincipalPosition = pair.principal.Position,
                        OrderAssociatedPosition = pair.associated.Position,

                        OrderAssociatedPositionNames = new
                        {
                            OrderNumber = query.For<Order>().Single(x => x.Id == pair.associated.Position.OrderId).Number,
                            OrderPositionName = query.For<Position>().Single(x => x.Id == pair.associated.Position.CausePackagePositionId).Name,
                            ItemPositionName = query.For<Position>().Single(x => x.Id == pair.associated.Position.CauseItemPositionId).Name,
                        },

                        OrderPrincipalPositionNames = new
                        {
                            OrderNumber = query.For<Order>().Single(x => x.Id == pair.principal.Position.OrderId).Number,
                            OrderPositionName = query.For<Position>().Single(x => x.Id == pair.principal.Position.PackagePositionId).Name,
                            ItemPositionName = query.For<Position>().Single(x => x.Id == pair.principal.Position.ItemPositionId).Name,
                        },
                    };

            var messages = from conflict in conflictingPositions
                           select new Version.ValidationResult
                           {
                               MessageParams =
                                    new XDocument(new XElement("root",
                                                               new XElement("firm",
                                                                            new XAttribute("id", conflict.FirmId)),
                                                               new XElement("position",
                                                                            new XAttribute("orderId", conflict.OrderAssociatedPosition.OrderId),
                                                                            new XAttribute("orderNumber", conflict.OrderAssociatedPositionNames.OrderNumber),
                                                                            new XAttribute("orderPositionId", conflict.OrderAssociatedPosition.CauseOrderPositionId),
                                                                            new XAttribute("orderPositionName", conflict.OrderAssociatedPositionNames.OrderPositionName),
                                                                            new XAttribute("positionId", conflict.OrderAssociatedPosition.CauseItemPositionId),
                                                                            new XAttribute("positionName", conflict.OrderAssociatedPositionNames.ItemPositionName)),
                                                               new XElement("position",
                                                                            new XAttribute("orderId", conflict.OrderPrincipalPosition.OrderId),
                                                                            new XAttribute("orderNumber", conflict.OrderPrincipalPositionNames.OrderNumber),
                                                                            new XAttribute("orderPositionId", conflict.OrderPrincipalPosition.OrderPositionId),
                                                                            new XAttribute("orderPositionName", conflict.OrderPrincipalPositionNames.OrderPositionName),
                                                                            new XAttribute("positionId", conflict.OrderPrincipalPosition.ItemPositionId),
                                                                            new XAttribute("positionName", conflict.OrderPrincipalPositionNames.ItemPositionName)),
                                                               new XElement("order",
                                                                            new XAttribute("id", conflict.OrderAssociatedPosition.OrderId),
                                                                            new XAttribute("number", conflict.OrderAssociatedPositionNames.OrderNumber)))),

                               PeriodStart = conflict.Start,
                               PeriodEnd = conflict.End,
                               ProjectId = conflict.ProjectId,

                               Result = RuleResult,
                           };

            return messages;
        }
    }
}
