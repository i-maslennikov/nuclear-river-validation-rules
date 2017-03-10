using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

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
    public sealed class FirmAssociatedPositionShouldNotStayAlone : ValidationResultAccessorBase
    {
        public FirmAssociatedPositionShouldNotStayAlone(IQuery query) : base(query, MessageTypeCode.FirmAssociatedPositionShouldNotStayAlone)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            // t.Principals.Any() выглядит избыточным при наличии  t.Principals.Select(x => x.OrderId).Distinct().Count() == 1, но обеспечивает ускорение выполнения запроса.
            var errors =
                query.For<Firm.FirmPosition>()
                     .Select(Specs.Join.Aggs.WithPrincipalPositions(query.For<Firm.FirmAssociatedPosition>(), query.For<Firm.FirmPosition>()))
                     .Where(dto => dto.Principals.Any() &&
                                   dto.Principals.Where(x => x.IsBindingObjectConditionSatisfied).All(x => x.Position.OrderId != dto.Associated.OrderId) &&
                                   dto.Principals.Where(x => x.IsBindingObjectConditionSatisfied).Select(x => x.Position.OrderId).Distinct().Count() == 1)
                     .Select(dto => new { associated = dto.Associated, principal = dto.Principals.First().Position });

            var messages =
                from error in errors
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(error.principal.OrderPositionId,
                                        new Reference<EntityTypeOrder>(error.principal.OrderId),
                                        new Reference<EntityTypePosition>(error.principal.PackagePositionId),
                                        new Reference<EntityTypePosition>(error.principal.ItemPositionId)),

                                    new Reference<EntityTypeOrderPosition>(error.associated.OrderPositionId,
                                        new Reference<EntityTypeOrder>(error.associated.OrderId),
                                        new Reference<EntityTypePosition>(error.associated.PackagePositionId),
                                        new Reference<EntityTypePosition>(error.associated.ItemPositionId)))
                                .ToXDocument(),

                        PeriodStart = error.principal.Begin,
                        PeriodEnd = error.principal.End,
                        OrderId = error.principal.OrderId,
                    };

            return messages;
        }
    }
}
