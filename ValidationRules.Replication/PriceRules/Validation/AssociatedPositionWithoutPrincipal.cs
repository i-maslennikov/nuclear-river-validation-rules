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
                select new Dto<Order.OrderPosition> { FirmId = order.FirmId, Start = period.Start, OrganizationUnitId = period.OrganizationUnitId, Scope = period.Scope, Position = position };

            var associatedPositions =
                from order in query.For<Order>()
                join period in query.For<Period.OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<Order.OrderAssociatedPosition>() on order.Id equals position.OrderId
                select new Dto<Order.OrderAssociatedPosition> { FirmId = order.FirmId, Start = period.Start, OrganizationUnitId = period.OrganizationUnitId, Scope = period.Scope, Position = position };

            var unsatisfiedPositions =
                associatedPositions.SelectMany(Specs.Join.Aggs.RegardlessBindingObject(orderPositions.DefaultIfEmpty()), Specs.Join.Aggs.RegardlessBindingObject())
                                   .GroupBy(x => new { x.Start, x.OrganizationUnitId, x.CausePosition.OrderId, x.CausePosition.PackagePositionId, x.CausePosition.ItemPositionId, x.CausePosition.OrderPositionId })
                                   .Where(group => group.Max(x => x.Match) == Match.NoPosition)
                                   .Select(group => group.Key);

            var messages =
                from unsatisfied in unsatisfiedPositions
                from period in query.For<Period>().Where(x => x.Start == unsatisfied.Start && x.OrganizationUnitId == unsatisfied.OrganizationUnitId)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(unsatisfied.OrderPositionId,
                                        new Reference<EntityTypeOrder>(unsatisfied.OrderId),
                                        new Reference<EntityTypePosition>(unsatisfied.PackagePositionId),
                                        new Reference<EntityTypePosition>(unsatisfied.ItemPositionId)))
                                .ToXDocument(),

                    PeriodStart = period.Start,
                    PeriodEnd = period.End,
                    OrderId = unsatisfied.OrderId,

                    Result = RuleResult,
                };

            return messages;
        }
    }
}
