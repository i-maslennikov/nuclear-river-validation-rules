using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Storage.Model.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.Actors.Validation
{
    /// <summary>
    /// Для заказов, которые содержат запрещённые друг к другу позиции должна выводиться ошибка
    /// (ошибка не должна выводиться, для одобренного заказа, если запрещённая позиция находится в не одобренном заказе)
    /// "{0} является запрещённой для: {1}"
    /// "{0} окажется запрещённой для: {1}"
    /// 
    /// Когда заказ переведён "на расторжение", он не должен мешать создать другой заказ с конфликтующей позицией, но возврат в размещение должно быть невозможно.
    /// </summary>
    public sealed class DeniedPositionsCheckActor : IActor
    {
        private const int MessageTypeId = 8;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        private readonly ValidationRuleShared _validationRuleShared;

        const int NoDependency = 2;
        const int Match = 1;
        const int Different = 3;

        private static readonly FindSpecification<AnonymousPositionType> NoDependencySpecification = new FindSpecification<AnonymousPositionType>(
            x => x.OrderDeniedPosition.BindingType == NoDependency);

        private static readonly FindSpecification<AnonymousPositionType> MatchSpecification = new FindSpecification<AnonymousPositionType>(
            x => x.OrderDeniedPosition.BindingType == Match &&
                 (x.OrderDeniedPosition.Category1Id == null || x.OrderDeniedPosition.Category1Id == x.OrderPosition.Category1Id) &&
                 (x.OrderDeniedPosition.Category3Id == null || x.OrderDeniedPosition.Category3Id == x.OrderPosition.Category3Id) &&
                 (x.OrderDeniedPosition.FirmAddressId == null || x.OrderDeniedPosition.FirmAddressId == x.OrderPosition.FirmAddressId));

        private static readonly FindSpecification<AnonymousPositionType> DifferentSpecification = new FindSpecification<AnonymousPositionType>(
            x => x.OrderDeniedPosition.BindingType == Different &&
                 (x.OrderDeniedPosition.Category1Id == null || x.OrderDeniedPosition.Category1Id != x.OrderPosition.Category1Id) &&
                 (x.OrderDeniedPosition.Category3Id == null || x.OrderDeniedPosition.Category3Id != x.OrderPosition.Category3Id) &&
                 (x.OrderDeniedPosition.FirmAddressId == null || x.OrderDeniedPosition.FirmAddressId != x.OrderPosition.FirmAddressId));

        private static readonly FindSpecification<AnonymousPositionType> BindingObjectSpecification =
            NoDependencySpecification | MatchSpecification | DifferentSpecification;

        public DeniedPositionsCheckActor(ValidationRuleShared validationRuleShared)
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
                from position in query.For<OrderPosition>()
                join order in query.For<Order>() on position.OrderId equals order.Id
                join op in query.For<OrderPeriod>() on order.Id equals op.OrderId
                select new { order.FirmId, op.Start, op.OrganizationUnitId, op.Scope, position };

            var deniedPositions =
                from position in query.For<OrderDeniedPosition>()
                join order in query.For<Order>() on position.OrderId equals order.Id
                join op in query.For<OrderPeriod>() on order.Id equals op.OrderId
                select new { order.FirmId, op.Start, op.OrganizationUnitId, op.Scope, position };

            var conflictsBeforeBindingObjectFilter =
                from orderPosition in orderPositions
                join deniedPosition in deniedPositions on
                    new { orderPosition.FirmId, orderPosition.Start, orderPosition.OrganizationUnitId, orderPosition.position.ItemPositionId } equals
                    new { deniedPosition.FirmId, deniedPosition.Start, deniedPosition.OrganizationUnitId, ItemPositionId = deniedPosition.position.DeniedPositionId }
                join period in query.For<Period>() on new { orderPosition.Start, orderPosition.OrganizationUnitId } equals new { period.Start, period.OrganizationUnitId }
                where orderPosition.position.OrderPositionId != deniedPosition.position.CauseOrderPositionId
                      && (orderPosition.Scope == 0 || orderPosition.Scope == deniedPosition.Scope)
                select new AnonymousPositionType
                    {
                        FirmId = orderPosition.FirmId,
                        Start = period.Start,
                        End = period.End,
                        ProjectId = period.ProjectId,
                        OrderPosition = orderPosition.position,
                        OrderDeniedPosition = deniedPosition.position,

                        OrderDeniedPositionNames = new AnonymousPositionType.NamesDto
                        {
                            OrderNumber = query.For<Order>().Single(x => x.Id == deniedPosition.position.OrderId).Number,
                            OrderPositionName = query.For<Position>().Single(x => x.Id == deniedPosition.position.CausePackagePositionId).Name,
                            ItemPositionName = query.For<Position>().Single(x => x.Id == deniedPosition.position.CauseItemPositionId).Name,
                        },

                        OrderPositionNames = new AnonymousPositionType.NamesDto
                        {
                            OrderNumber = query.For<Order>().Single(x => x.Id == orderPosition.position.OrderId).Number,
                            OrderPositionName = query.For<Position>().Single(x => x.Id == orderPosition.position.PackagePositionId).Name,
                            ItemPositionName = query.For<Position>().Single(x => x.Id == orderPosition.position.ItemPositionId).Name,
                        },
                };

            var messages =
                from conflict in conflictsBeforeBindingObjectFilter.Where(BindingObjectSpecification)
                select new Version.ValidationResult
                    {
                        VersionId = version,

                        MessageType = MessageTypeId,
                        MessageParams =
                            new XDocument(new XElement("element",
                                                       new XAttribute("firm", conflict.FirmId),
                                                       new XElement("position",
                                                                    new XAttribute("orderId", conflict.OrderDeniedPosition.OrderId),
                                                                    new XAttribute("orderNumber", conflict.OrderDeniedPositionNames.OrderNumber),
                                                                    new XAttribute("orderPositionId", conflict.OrderDeniedPosition.CauseOrderPositionId),
                                                                    new XAttribute("orderPositionName", conflict.OrderDeniedPositionNames.OrderPositionName),
                                                                    new XAttribute("positionId", conflict.OrderDeniedPosition.CauseItemPositionId),
                                                                    new XAttribute("positionName", conflict.OrderDeniedPositionNames.ItemPositionName)),
                                                       new XElement("position",
                                                                    new XAttribute("orderId", conflict.OrderPosition.OrderId),
                                                                    new XAttribute("orderNumber", conflict.OrderPositionNames.OrderNumber),
                                                                    new XAttribute("orderPositionId", conflict.OrderPosition.OrderPositionId),
                                                                    new XAttribute("orderPositionName", conflict.OrderPositionNames.OrderPositionName),
                                                                    new XAttribute("positionId", conflict.OrderPosition.ItemPositionId),
                                                                    new XAttribute("positionName", conflict.OrderPositionNames.ItemPositionName)))),
                        PeriodStart = conflict.Start,
                        PeriodEnd = conflict.End,
                        ProjectId = conflict.ProjectId,

                        ReferenceType = EntityTypeIds.Order,
                        ReferenceId = conflict.OrderDeniedPosition.OrderId,

                        Result = RuleResult,
                    };

            return messages;
        }

        class AnonymousPositionType
        {
            public long FirmId { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public long ProjectId { get; set; }
            public OrderPosition OrderPosition { get; set; }
            public OrderDeniedPosition OrderDeniedPosition { get; set; }
            public NamesDto OrderPositionNames { get; set; }
            public NamesDto OrderDeniedPositionNames { get; set; }

            public class NamesDto
            {
                public string OrderNumber { get; set; }
                public string OrderPositionName { get; set; }
                public string ItemPositionName { get; set; }
            }
        }
    }
}
