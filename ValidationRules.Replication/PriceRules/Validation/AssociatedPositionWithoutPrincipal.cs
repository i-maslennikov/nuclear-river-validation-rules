using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказа, в котором есть сопутствующая позиция и нет основной должна выводиться ошибка.
    /// "{0} является сопутствующей, основная позиция не найдена"
    /// 
    /// Source: AssociatedAndDeniedPricePositionsOrderValidationRule/AssociatedPositionWithoutPrincipalTemplate
    /// 
    /// Q1: Может ли элемент пакета быть удовлетворён другим элементом того же самого пакета?
    /// 
    /// Q2: Может ли пакет быть удовлетворён своим элементом или наоборот?
    /// </summary>
    // todo: переименовать PrincipalPositionMustExistForAssociated
    public sealed class AssociatedPositionWithoutPrincipal : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenSingleForApprove(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public AssociatedPositionWithoutPrincipal(IQuery query) : base(query, MessageTypeCode.AssociatedPositionWithoutPrincipal)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var orderPositions =
                from order in query.For<Order>()
                join period in query.For<Period.OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<Order.OrderPosition>() on order.Id equals position.OrderId
                select new { order.FirmId, period.Start, period.OrganizationUnitId, period.Scope, position };

            var associatedPositions =
                from order in query.For<Order>()
                join period in query.For<Period.OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<Order.OrderAssociatedPosition>() on order.Id equals position.OrderId
                select new { order.FirmId, period.Start, period.OrganizationUnitId, period.Scope, position };

            var satisfiedPositions =
                from orderPosition in orderPositions
                join associatedPosition in associatedPositions on
                    new { orderPosition.FirmId, orderPosition.Start, orderPosition.OrganizationUnitId, orderPosition.position.ItemPositionId } equals
                    new { associatedPosition.FirmId, associatedPosition.Start, associatedPosition.OrganizationUnitId, ItemPositionId = associatedPosition.position.PrincipalPositionId }
                where Scope.CanSee(associatedPosition.Scope, orderPosition.Scope)
                select new
                    {
                        associatedPosition.Start,
                        associatedPosition.position.CauseOrderPositionId,
                        associatedPosition.position.CauseItemPositionId,
                    };

            var notSatisfiedPositions =
                from position in associatedPositions
                from satisfied in satisfiedPositions.Where(x => x.CauseOrderPositionId == position.position.CauseOrderPositionId &&
                                                                x.CauseItemPositionId == position.position.CauseItemPositionId &&
                                                                x.Start == position.Start).DefaultIfEmpty()
                where satisfied == null
                select new
                    {
                        position.FirmId,
                        position.position.OrderId,
                        position.position.CauseOrderPositionId,
                        position.position.CausePackagePositionId,
                        position.position.CauseItemPositionId,

                        position.Start,
                        position.OrganizationUnitId,
                    };

            var messages =
                from conflict in notSatisfiedPositions.Distinct()
                join period in query.For<Period>() on new { conflict.Start, conflict.OrganizationUnitId } equals new { period.Start, period.OrganizationUnitId }
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(conflict.CauseOrderPositionId,
                                        new Reference<EntityTypeOrder>(conflict.OrderId),
                                        new Reference<EntityTypePosition>(conflict.CausePackagePositionId),
                                        new Reference<EntityTypePosition>(conflict.CauseItemPositionId)))
                                .ToXDocument(),

                        PeriodStart = period.Start,
                        PeriodEnd = period.End,
                        OrderId = conflict.OrderId,

                        Result = RuleResult,
                    };

            return messages;
        }
    }
}
