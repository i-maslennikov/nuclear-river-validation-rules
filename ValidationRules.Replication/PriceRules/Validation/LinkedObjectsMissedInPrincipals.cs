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
        private const int BindingTypeMatch = 1;

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
            var orderPositions =
                from order in query.For<Order>()
                join period in query.For<Period.OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<Order.OrderPosition>() on order.Id equals position.OrderId
                select new Dto<Order.OrderPosition> { FirmId = order.FirmId, Start = period.Start, OrganizationUnitId = period.OrganizationUnitId, Scope = period.Scope, Position = position };

            var associatedPositions =
                from order in query.For<Order>()
                join period in query.For<Period.OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<Order.OrderAssociatedPosition>() on order.Id equals position.OrderId
                where position.BindingType == BindingTypeMatch // небольшой косяк (который есть и в erm) - если сопутствующая удовлетворена мастер-позицией (другой) без учёта привязки, то эта проверка выдаст ошибку, т.е получается как бы эмуляция двух групп основных позиций.
                select new Dto<Order.OrderAssociatedPosition> { FirmId = order.FirmId, Start = period.Start, OrganizationUnitId = period.OrganizationUnitId, Scope = period.Scope, Position = position };

            // Есть DefaultIfEmpty или нет - с точки зрения логики без разницы, но left join работает быстрее 0_0
            var unsatisfiedPositions =
                associatedPositions.SelectMany(Specs.Join.Aggs.RegardlessBindingObject(orderPositions.DefaultIfEmpty()), Specs.Join.Aggs.RegardlessBindingObject())
                                   .GroupBy(x => new { x.Start, x.OrganizationUnitId, x.CausePosition.OrderId, x.CausePosition.PackagePositionId, x.CausePosition.ItemPositionId, x.CausePosition.OrderPositionId })
                                   .Where(group => group.Max(x => x.Match) == Match.DifferentBindingObject)
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

                        PeriodStart = unsatisfied.Start,
                        PeriodEnd = period.End,
                        OrderId = unsatisfied.OrderId,
                        ProjectId = null,

                        Result = RuleResult,
                    };

            return messages;
        }
    }
}
