using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
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
    /// Source: ADP/ConflictingPrincipalPosition
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
    /// </summary>
    public sealed class ConflictingPrincipalPositionActor : IActor
    {
        private const int Different = 3;

        private const int MessageTypeId = 11;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        private readonly ValidationRuleShared _validationRuleShared;

        public ConflictingPrincipalPositionActor(ValidationRuleShared validationRuleShared)
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

            var conflictingPositions =
                from associatedPosition in associatedPositions
                join principalPosition in orderPositions on
                    new { associatedPosition.FirmId, associatedPosition.Start, associatedPosition.OrganizationUnitId, ItemPositionId = associatedPosition.position.PrincipalPositionId } equals
                    new { principalPosition.FirmId, principalPosition.Start, principalPosition.OrganizationUnitId, principalPosition.position.ItemPositionId }
                join period in query.For<Period>() on new { principalPosition.Start, principalPosition.OrganizationUnitId } equals new { period.Start, period.OrganizationUnitId }
                where
                    principalPosition.position.OrderPositionId != associatedPosition.position.CauseOrderPositionId && (principalPosition.Scope == 0 || principalPosition.Scope == associatedPosition.Scope)
                select new PrincipalAssociatedPostionPair
                    {
                        FirmId = principalPosition.FirmId,
                        Start = period.Start,
                        End = period.End,
                        ProjectId = period.ProjectId,
                        OrderPrincipalPosition = principalPosition.position,
                        OrderAssociatedPosition = associatedPosition.position,

                        OrderAssociatedPositionNames = new PrincipalAssociatedPostionPair.NamesDto
                        {
                            OrderNumber = query.For<Order>().Single(x => x.Id == associatedPosition.position.OrderId).Number,
                            OrderPositionName = query.For<Position>().Single(x => x.Id == associatedPosition.position.CausePackagePositionId).Name,
                            ItemPositionName = query.For<Position>().Single(x => x.Id == associatedPosition.position.CauseItemPositionId).Name,
                        },

                        OrderPrincipalPositionNames = new PrincipalAssociatedPostionPair.NamesDto
                        {
                            OrderNumber = query.For<Order>().Single(x => x.Id == principalPosition.position.OrderId).Number,
                            OrderPositionName = query.For<Position>().Single(x => x.Id == principalPosition.position.PackagePositionId).Name,
                            ItemPositionName = query.For<Position>().Single(x => x.Id == principalPosition.position.ItemPositionId).Name,
                        },
                    };

            var messages = from conflict in conflictingPositions.Where(x => x.OrderAssociatedPosition.BindingType == Different).Where(Specs.Find.Aggs.BindingObjectMatch())
                           select new Version.ValidationResult
                           {
                               VersionId = version,
                               MessageType = MessageTypeId,
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

                               ReferenceType = EntityTypeIds.Order,
                               ReferenceId = conflict.OrderAssociatedPosition.OrderId,

                               Result = RuleResult,
                           };

            return messages;
        }
    }
}
