using System;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.PriceRules.Validation.Dto;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказов, которые содержат запрещённые друг к другу позиции должна выводиться ошибка
    /// (ошибка не должна выводиться, для одобренного заказа, если запрещённая позиция находится в не одобренном заказе)
    /// "{0} является запрещённой для: {1}"
    /// "{0} окажется запрещённой для: {1}"
    /// 
    /// Source: AssociatedAndDeniedPricePositionsOrderValidationRule/ADPCheckModeSpecificOrder_MessageTemplate
    ///         AssociatedAndDeniedPricePositionsOrderValidationRule/ADPCheckModeOrderBeingReapproved_MessageTemplate
    /// Когда заказ переведён "на расторжение", он не должен мешать создать другой заказ с конфликтующей позицией, но возврат в размещение должно быть невозможно.
    /// </summary>
    public sealed class DeniedPositionsCheck : ValidationResultAccessorBase
    {
        private const int NoDependency = 2;
        private const int Match = 1;
        private const int Different = 3;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        private static readonly Expression<Func<Dto<OrderDeniedPosition>, Dto<OrderPosition>, PrincipalDeniedDto>> PrincipalDeniedProjection =
            (denied, principal) => new PrincipalDeniedDto
                {
                    FirmId = denied.FirmId,
                    OrganizationUnitId = denied.OrganizationUnitId,
                    Start = denied.Start,

                    DeniedOrderId = denied.Position.OrderId,
                    DeniedCauseOrderPositionId = denied.Position.CauseOrderPositionId,
                    DeniedCausePackagePositionId = denied.Position.CausePackagePositionId,
                    DeniedCauseItemPositionId = denied.Position.CauseItemPositionId,

                    PrincipalOrderId = principal.Position.OrderId,
                    PrincipalOrderPositionId = principal.Position.OrderPositionId,
                    PrincipalPackagePositionId = principal.Position.PackagePositionId,
                    PrincipalItemPositionId = principal.Position.ItemPositionId,
                };

        public DeniedPositionsCheck(IQuery query) : base(query, MessageTypeCode.DeniedPositionsCheck)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var orderPositions =
                from position in query.For<OrderPosition>()
                join order in query.For<Order>() on position.OrderId equals order.Id
                join period in query.For<OrderPeriod>() on order.Id equals period.OrderId
                select new Dto<OrderPosition> { FirmId = order.FirmId, Start = period.Start, OrganizationUnitId = period.OrganizationUnitId, Scope = period.Scope, Position = position };

            var deniedPositions =
                from position in query.For<OrderDeniedPosition>()
                join order in query.For<Order>() on position.OrderId equals order.Id
                join period in query.For<OrderPeriod>() on order.Id equals period.OrderId
                select new Dto<OrderDeniedPosition> { FirmId = order.FirmId, Start = period.Start, OrganizationUnitId = period.OrganizationUnitId, Scope = period.Scope, Position = position };

            var match = deniedPositions.Where(x => x.Position.BindingType == Match)
                                       .SelectMany(Specs.Join.Aggs.DeniedWithMatchedBindingObject(orderPositions), PrincipalDeniedProjection);

            var differ = deniedPositions.Where(x => x.Position.BindingType == Different)
                                       .SelectMany(Specs.Join.Aggs.DeniedWithDifferentBindingObject(orderPositions), PrincipalDeniedProjection);

            var noMatter = deniedPositions.Where(x => x.Position.BindingType == NoDependency)
                                          .SelectMany(Specs.Join.Aggs.DeniedWithoutConsideringBindingObject(orderPositions), PrincipalDeniedProjection);

            var messages =
                from conflict in match.Union(differ).Union(noMatter)
                join period in query.For<Period>() on new { conflict.Start, conflict.OrganizationUnitId } equals new { period.Start, period.OrganizationUnitId }
                let names = new
                    {
                        DeniedPosition = new
                            {
                                OrderNumber = query.For<Order>().Single(x => x.Id == conflict.DeniedOrderId).Number,
                                OrderPositionName = query.For<Position>().Single(x => x.Id == conflict.DeniedCausePackagePositionId).Name,
                                ItemPositionName = query.For<Position>().Single(x => x.Id == conflict.DeniedCauseItemPositionId).Name,
                            },

                        OrderPosition = new
                            {
                                OrderNumber = query.For<Order>().Single(x => x.Id == conflict.PrincipalOrderId).Number,
                                OrderPositionName = query.For<Position>().Single(x => x.Id == conflict.PrincipalPackagePositionId).Name,
                                ItemPositionName = query.For<Position>().Single(x => x.Id == conflict.PrincipalItemPositionId).Name,
                            },
                    }
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new XDocument(new XElement("root",
                                new XElement("firm",
                                    new XAttribute("id", conflict.FirmId)),
                                new XElement("position",
                                    new XAttribute("orderId", conflict.DeniedOrderId),
                                    new XAttribute("orderNumber", names.DeniedPosition.OrderNumber),
                                    new XAttribute("orderPositionId", conflict.DeniedCauseOrderPositionId),
                                    new XAttribute("orderPositionName", names.DeniedPosition.OrderPositionName),
                                    new XAttribute("positionId", conflict.DeniedCauseItemPositionId),
                                    new XAttribute("positionName", names.DeniedPosition.ItemPositionName)),
                                new XElement("position",
                                    new XAttribute("orderId", conflict.PrincipalOrderId),
                                    new XAttribute("orderNumber", names.OrderPosition.OrderNumber),
                                    new XAttribute("orderPositionId", conflict.PrincipalOrderPositionId),
                                    new XAttribute("orderPositionName", names.OrderPosition.OrderPositionName),
                                    new XAttribute("positionId", conflict.PrincipalItemPositionId),
                                    new XAttribute("positionName", names.OrderPosition.ItemPositionName)),
                                new XElement("order",
                                    new XAttribute("id", conflict.DeniedOrderId),
                                    new XAttribute("number", names.DeniedPosition.OrderNumber)))),
                        PeriodStart = period.Start,
                        PeriodEnd = period.End,
                        OrderId = conflict.DeniedOrderId,

                        Result = RuleResult,
                    };

            return messages;
        }

        /// <summary>
        /// Бага в Union требует plain-dto, без составных полей.
        /// </summary>
        private sealed class PrincipalDeniedDto
        {
            public long FirmId { get; set; }
            public DateTime Start { get; set; }
            public long OrganizationUnitId { get; set; }

            public long PrincipalOrderId { get; set; }
            public long PrincipalOrderPositionId { get; set; }
            public long PrincipalPackagePositionId { get; set; }
            public long PrincipalItemPositionId { get; set; }

            public long DeniedOrderId { get; set; }
            public long DeniedCauseOrderPositionId { get; set; }
            public long DeniedCausePackagePositionId { get; set; }
            public long DeniedCauseItemPositionId { get; set; }
        }
    }
}
