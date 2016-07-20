using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.PriceRules.Validation.Dto;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказа, в котором есть сопутствующая позиция и есть основная, но не удовлетворено условие ObjectBindingType.Match должна выводиться ошибка.
    /// "{0} содержит объекты привязки, отсутствующие в основных позициях"
    /// 
    /// Source: ADP/LinkedObjectsMissedInPrincipals
    /// 
    /// Q: Позиция Y - сопутствующая для X (с требованием, чтобы объекты привязки совпадали).
    ///    Продана X в рубрику адреса (A, B). Продана Y в рубрику B. Должна ли появиться ошибка?
    /// A: Завит от того, как заполнена таблица сравнения объектов привязки. По той, что у нас сейчас - нет. В реальности - по-разному.
    /// 
    /// Q: Позиция Y - сопутствующая для X (с требованием, чтобы объекты привязки совпадали).
    ///    Продана X в рубрику адреса (A, B). Продана Y к адресу A. Должна ли появиться ошибка?
    /// A: Завит от того, как заполнена таблица сравнения объектов привязки. По той, что у нас сейчас - нет. В реальности - по-разному.
    /// 
    /// Q: Позиция Z - сопутствующая для X (без учёта), для Y (совпадение).
    ///    Проданы Z, Y (с другим объектом привязки), X. Должна ли появиться ошибка?
    /// A: Сейчас ERM выдаёт ошибку. Если удалить позицию Y из заказа, ошибка остаётся - позиции "без учёта" не могут удовлетворить это правило.
    /// 
    /// Q: Позиция Y - сопутствующая для X (совпадение)
    ///    Y продана в рубрики A и B. X продана только в A. Должна ли появиться ошибка?
    /// A: Да.
    /// 
    /// Q: Позиция Z - сопутствующая для X, Y (совпадение)
    ///    Z продана в A, B. X продана в A. Y продана в B. Должна ли появиться ошибка?
    /// A: Нет.
    /// </summary>
    public sealed class LinkedObjectsMissedInPrincipalsActor : IActor
    {
        private const int Match = 1;

        // TODO: можно вполне выводить в какой именно основной позиции отсутствуют объекты привязки, но в ERM так не делают, и мы не будем
        private const int MessageTypeId = 10;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        private readonly ValidationRuleShared _validationRuleShared;

        public LinkedObjectsMissedInPrincipalsActor(ValidationRuleShared validationRuleShared)
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
                from order in query.For<Order>()
                join period in query.For<OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<OrderPosition>() on order.Id equals position.OrderId
                select new Dto<OrderPosition> { FirmId = order.FirmId, Start = period.Start, OrganizationUnitId = period.OrganizationUnitId, Scope = period.Scope, Position = position };

            var associatedPositions =
                from order in query.For<Order>()
                join period in query.For<OrderPeriod>() on order.Id equals period.OrderId
                join position in query.For<OrderAssociatedPosition>() on order.Id equals position.OrderId
                where position.BindingType == Match // небольшой косяк (который есть и в erm) - если сопутствующая удовлетворена мастер-позицией без учёта привязки, то эта проверка выдаст ошибку.
                select new Dto<OrderAssociatedPosition> { FirmId = order.FirmId, Start = period.Start, OrganizationUnitId = period.OrganizationUnitId, Scope = period.Scope, Position = position };

            var unsatisfiedPositions =
                associatedPositions.SelectMany(Specs.Join.Aggs.WithMatchedBindingObject(orderPositions.DefaultIfEmpty()), (associated, principal) => new { associated, principal })
                                   .GroupBy(x => new
                                   {
                                       // можно включать все поля, какие захотим иметь в выборке, кроме двух: PrincipalPositionId, Source
                                       Start = x.associated.Start,
                                       Scope = x.associated.Scope,
                                       Category1Id = x.associated.Position.Category1Id,
                                       Category3Id = x.associated.Position.Category3Id,
                                       CauseItemPositionId = x.associated.Position.CauseItemPositionId,
                                       CauseOrderPositionId = x.associated.Position.CauseOrderPositionId,
                                       CausePackagePositionId = x.associated.Position.CausePackagePositionId,
                                       FirmAddressId = x.associated.Position.FirmAddressId,
                                       FirmId = x.associated.FirmId,
                                       OrganizationUnitId = x.associated.OrganizationUnitId,
                                       OrderId = x.associated.Position.OrderId,
                                   })
                                   .Where(x => x.All(y => y.principal == null))
                                   .Select(grouping => new
                                   {
                                       grouping.Key,

                                       ProjectId = query.For<Period>().Single(x => x.Start == grouping.Key.Start && x.OrganizationUnitId == grouping.Key.OrganizationUnitId).ProjectId,
                                       End = query.For<Period>().Single(x => x.Start == grouping.Key.Start && x.OrganizationUnitId == grouping.Key.OrganizationUnitId).End,
                                       OrderNumber = query.For<Order>().Single(x => x.Id == grouping.Key.OrderId).Number,
                                       OrderPositionName = query.For<Position>().Single(x => x.Id == grouping.Key.CausePackagePositionId).Name,
                                       ItemPositionName = query.For<Position>().Single(x => x.Id == grouping.Key.CauseItemPositionId).Name,
                                   });

            var messages = from unsatisfied in unsatisfiedPositions
                           select new Version.ValidationResult
                           {
                               VersionId = version,
                               MessageType = MessageTypeId,
                               MessageParams =
                                    new XDocument(new XElement("root",
                                                               new XElement("firm",
                                                                            new XAttribute("id", unsatisfied.Key.FirmId)),
                                                               new XElement("position",
                                                                            new XAttribute("orderId", unsatisfied.Key.OrderId),
                                                                            new XAttribute("orderNumber", unsatisfied.OrderNumber),
                                                                            new XAttribute("orderPositionId", unsatisfied.Key.CauseOrderPositionId),
                                                                            new XAttribute("orderPositionName", unsatisfied.OrderPositionName),
                                                                            new XAttribute("positionId", unsatisfied.Key.CauseItemPositionId),
                                                                            new XAttribute("positionName", unsatisfied.ItemPositionName)),
                                                               new XElement("order",
                                                                            new XAttribute("id", unsatisfied.Key.OrderId),
                                                                            new XAttribute("number", unsatisfied.OrderNumber)))),

                               PeriodStart = unsatisfied.Key.Start,
                               PeriodEnd = unsatisfied.End,
                               ProjectId = unsatisfied.ProjectId,

                               ReferenceType = EntityTypeIds.Order,
                               ReferenceId = unsatisfied.Key.OrderId,

                               Result = RuleResult,
                           };

            return messages;
        }
    }
}
