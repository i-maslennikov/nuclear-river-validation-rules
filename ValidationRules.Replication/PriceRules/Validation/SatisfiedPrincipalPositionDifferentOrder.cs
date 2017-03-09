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
    public sealed class SatisfiedPrincipalPositionDifferentOrder : ValidationResultAccessorBase2
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingleForCancel(Result.Warning);

        public SatisfiedPrincipalPositionDifferentOrder(IQuery query) : base(query, MessageTypeCode.SatisfiedPrincipalPositionDifferentOrder)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var warnings =
                query.For<Firm.FirmPosition>().Select(associated => new
                         {
                             associated,
                             principals = (query.For<Firm.FirmAssociatedPosition>()
                                                .Where(x => x.OrderPositionId == associated.OrderPositionId && x.ItemPositionId == associated.ItemPositionId)
                                                .SelectMany(requirement => query.For<Firm.FirmPosition>()
                                                                                .Where(x => x.Begin == associated.Begin && x.FirmId == associated.FirmId)
                                                                                .Where(x => x.ItemPositionId == requirement.PrincipalPositionId && x.FirmId == requirement.FirmId)
                                                                                .Where(principal => Scope.CanSee(associated.Scope, principal.Scope))
                                                                                .Where(principal => requirement.BindingType == 2 || ((principal.HasNoBinding == associated.HasNoBinding) &&
                                                                                                     ((associated.Category3Id != null &&
                                                                                                       associated.Category3Id == principal.Category3Id &&
                                                                                                       (associated.FirmAddressId == null ||
                                                                                                        principal.FirmAddressId == null)) ||
                                                                                                      ((associated.Category1Id == null ||
                                                                                                        principal.Category1Id == null ||
                                                                                                        associated.Category3Id == principal.Category3Id ||
                                                                                                        associated.Category1Id == principal.Category1Id &&
                                                                                                        associated.Category3Id == null &&
                                                                                                        principal.Category3Id == null) &&
                                                                                                       associated.FirmAddressId == principal.FirmAddressId))
                                                                                                        ? requirement.BindingType == 1
                                                                                                        : requirement.BindingType == 3))))
                         })
                     .Where(@t => @t.principals.Any() && @t.principals.All(x => x.OrderId != @t.associated.OrderId) && @t.principals.Select(x => x.OrderId).Distinct().Count() == 1)
                     .Select(@t => new { @t.associated, principal = @t.principals.First() });

            var messages =
                from warning in warnings
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(warning.principal.OrderPositionId,
                                        new Reference<EntityTypeOrder>(warning.principal.OrderId),
                                        new Reference<EntityTypePosition>(warning.principal.PackagePositionId),
                                        new Reference<EntityTypePosition>(warning.principal.ItemPositionId)),

                                    new Reference<EntityTypeOrderPosition>(warning.associated.OrderPositionId,
                                        new Reference<EntityTypeOrder>(warning.associated.OrderId),
                                        new Reference<EntityTypePosition>(warning.associated.PackagePositionId),
                                        new Reference<EntityTypePosition>(warning.associated.ItemPositionId)))
                                .ToXDocument(),

                        PeriodStart = warning.principal.Begin,
                        PeriodEnd = warning.principal.End,
                        OrderId = warning.principal.OrderId,

                        Result = RuleResult,
                    };

            var xxx = messages.ToString();
            return messages;
        }
    }
}
