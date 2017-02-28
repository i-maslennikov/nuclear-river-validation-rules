using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.PriceRules.Validation.Dto;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказов в статусе "на расторжении", если они содержат основную позицию для остающихся одобренными заказов и среди одобренных нет другой основной позиции должно выводиться предупреждение.
    /// Позиция {0} данного Заказа является основной для следующих позиций:
    /// Позиция {0} Заказа {1}
    /// 
    /// Source: AssociatedAndDeniedPricePositionsOrderValidationRule/ADPValidation_Template
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
    public sealed class SatisfiedPrincipalPositionDifferentOrder : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingleForCancel(Result.Warning);

        public SatisfiedPrincipalPositionDifferentOrder(IQuery query) : base(query, MessageTypeCode.SatisfiedPrincipalPositionDifferentOrder)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var orderPositions =
                from order in query.For<Order>()
                join period in query.For<Period.OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<Order.OrderPosition>() on order.Id equals position.OrderId
                select new Dto<Order.OrderPosition> { FirmId = order.FirmId, Start = period.Start, OrganizationUnitId = period.OrganizationUnitId, Scope = period.Scope, Position = position };

            var associatedPositions =
                from order in query.For<Order>()
                join period in query.For<Period.OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<Order.OrderAssociatedPosition>() on order.Id equals position.OrderId
                where period.Scope == 0
                select new Dto<Order.OrderAssociatedPosition> { FirmId = order.FirmId, Start = period.Start, OrganizationUnitId = period.OrganizationUnitId, Scope = period.Scope, Position = position };

            var notSatisfiedPositions =
                associatedPositions.SelectMany(Specs.Join.Aggs.AvailablePrincipalPosition(orderPositions.Where(x => x.Scope == 0).DefaultIfEmpty()), (associated, principal) => new { associated, has = principal != null })
                                   .GroupBy(x => new
                                       {
                                           x.associated.Start,
                                           x.associated.Position.OrderId,
                                           x.associated.Position.CauseOrderPositionId,
                                           x.associated.Position.CauseItemPositionId,
                                           x.associated.Position.Category1Id,
                                           x.associated.Position.Category3Id,
                                           x.associated.Position.FirmAddressId,
                                       })
                                   .Where(x => x.Max(y => y.has) == false)
                                   .Select(x => x.Key);

            var satisfiedOnlyByHiddenPositions =
                associatedPositions.SelectMany(Specs.Join.Aggs.AvailablePrincipalPosition(orderPositions.Where(x => x.Scope != 0)), (associated, principal) => new { associated, principal })
                                   .Where(x => notSatisfiedPositions.Any(y => y.Start == x.associated.Start &&
                                                                              y.CauseOrderPositionId == x.associated.Position.CauseOrderPositionId &&
                                                                              y.CauseItemPositionId == x.associated.Position.CauseItemPositionId));

            var messages =
                from warning in satisfiedOnlyByHiddenPositions
                join period in query.For<Period>() on new { warning.principal.Start, warning.principal.OrganizationUnitId } equals new { period.Start, period.OrganizationUnitId }
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(warning.principal.Position.OrderPositionId,
                                        new Reference<EntityTypeOrder>(warning.principal.Position.OrderId),
                                        new Reference<EntityTypePosition>(warning.principal.Position.PackagePositionId),
                                        new Reference<EntityTypePosition>(warning.principal.Position.ItemPositionId)),

                                    new Reference<EntityTypeOrderPosition>(warning.associated.Position.CauseOrderPositionId,
                                        new Reference<EntityTypeOrder>(warning.associated.Position.OrderId),
                                        new Reference<EntityTypePosition>(warning.associated.Position.CausePackagePositionId),
                                        new Reference<EntityTypePosition>(warning.associated.Position.CauseItemPositionId)))
                                .ToXDocument(),

                        PeriodStart = period.Start,
                        PeriodEnd = period.End,
                        OrderId = warning.principal.Position.OrderId,

                        Result = RuleResult,
                    };

            return messages;
        }
    }
}
