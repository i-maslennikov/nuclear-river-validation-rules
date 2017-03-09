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
    /// Для заказа, в котором есть сопутствующая позиция и есть основная, но не удовлетворено условие ObjectBindingType.Match должна выводиться ошибка.
    /// "{0} содержит объекты привязки, отсутствующие в основных позициях"
    /// 
    /// Source: AssociatedAndDeniedPricePositionsOrderValidationRule/LinkedObjectsMissedInPrincipals
    /// 
    /// Q1: Позиция Y - сопутствующая для X (с требованием, чтобы объекты привязки совпадали).
    ///     Продана X в рубрику адреса (A, B). Продана Y в рубрику B. Должна ли появиться ошибка?
    /// A: Завит от того, как заполнена таблица сравнения объектов привязки. По той, что у нас сейчас - нет. В реальности - по-разному.
    /// 
    /// Q2: Позиция Y - сопутствующая для X (с требованием, чтобы объекты привязки совпадали).
    ///     Продана X в рубрику адреса (A, B). Продана Y к адресу A. Должна ли появиться ошибка?
    /// A: Завит от того, как заполнена таблица сравнения объектов привязки. По той, что у нас сейчас - нет. В реальности - по-разному.
    /// 
    /// Q3: Позиция Z - сопутствующая для X (без учёта), для Y (совпадение).
    ///     Проданы Z, Y (с другим объектом привязки), X. Должна ли появиться ошибка?
    /// A: Сейчас ERM выдаёт ошибку. Если удалить позицию Y из заказа, ошибка остаётся - позиции "без учёта" не могут удовлетворить это правило.
    /// 
    /// Q4: Позиция Y - сопутствующая для X (совпадение)
    ///     Y продана в рубрики A и B. X продана только в A. Должна ли появиться ошибка?
    /// A: Да.
    /// 
    /// Q5: Позиция Z - сопутствующая для X, Y (совпадение)
    ///     Z продана в A, B. X продана в A. Y продана в B. Должна ли появиться ошибка?
    /// A: Нет.
    /// 
    /// TODO: можно вполне выводить в какой именно основной позиции отсутствуют объекты привязки, но в ERM так не делают, и мы не будем
    /// </summary>
    // todo: переименовать PrincipalPositionMustHaveSameBindingObject
    public sealed class LinkedObjectsMissedInPrincipals : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenSingleForApprove(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public LinkedObjectsMissedInPrincipals(IQuery query) : base(query, MessageTypeCode.LinkedObjectsMissedInPrincipals)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var errors =
                query.For<Firm.FirmPosition>()
                     .Select(Specs.Join.Aggs.WithPrincipalPositions(query.For<Firm.FirmAssociatedPosition>(), query.For<Firm.FirmPosition>()))
                     .Where(dto => !dto.Principals.Any(x => x.IsBindingObjectConditionSatisfied) && dto.Principals.Any(x => x.RequiredMatch))
                     .Select(dto => dto.Associated);

            var messages =
                from error in errors
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(error.OrderPositionId,
                                        new Reference<EntityTypeOrder>(error.OrderId),
                                        new Reference<EntityTypePosition>(error.PackagePositionId),
                                        new Reference<EntityTypePosition>(error.ItemPositionId)))
                                .ToXDocument(),

                        PeriodStart = error.Begin,
                        PeriodEnd = error.End,
                        OrderId = error.OrderId,
                        ProjectId = null,

                        Result = RuleResult,
                    };

            return messages;
        }
    }
}
