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
    // todo: переименовать PrincipalPositionMustHaveDifferentBindingObject
    // todo: проверка не срабатывает - похоже, продажи невозможны
    public sealed class ConflictingPrincipalPosition : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenSingleForApprove(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public ConflictingPrincipalPosition(IQuery query) : base(query, MessageTypeCode.ConflictingPrincipalPosition)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var errors =
                query.For<Firm.FirmPosition>()
                     .SelectMany(Specs.Join.Aggs.PrincipalPositions(query.For<Firm.FirmAssociatedPosition>(), query.For<Firm.FirmPosition>()), (associated, principal) => new { associated, principal })
                     .Where(dto => dto.principal.RequiredDifferent && !dto.principal.IsBindingObjectConditionSatisfied)
                     .Select(dto => new { dto.associated, principal = dto.principal.Position });

            var messages =
                from error in errors
                select new Version.ValidationResult
                {
                    MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(error.associated.OrderPositionId,
                                        new Reference<EntityTypeOrder>(error.associated.OrderId),
                                        new Reference<EntityTypePosition>(error.associated.PackagePositionId),
                                        new Reference<EntityTypePosition>(error.associated.ItemPositionId)),

                                    new Reference<EntityTypeOrderPosition>(error.principal.OrderPositionId,
                                        new Reference<EntityTypeOrder>(error.principal.OrderId),
                                        new Reference<EntityTypePosition>(error.principal.PackagePositionId),
                                        new Reference<EntityTypePosition>(error.principal.ItemPositionId)))
                                .ToXDocument(),

                    PeriodStart = error.associated.Begin,
                    PeriodEnd = error.associated.End,
                    OrderId = error.associated.OrderId,

                    Result = RuleResult,
                };

            return messages;
        }
    }
}
