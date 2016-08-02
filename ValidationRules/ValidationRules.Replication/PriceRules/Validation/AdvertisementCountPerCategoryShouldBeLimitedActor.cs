using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для проектов, с превышением продаж определённой номенклатуры в рубрику, должна выводиться ошибка в массовой и предупреждение в единичной.
    /// "В рубрику {0} заказано слишком много объявлений: Заказано {1}, допустимо не более {2}"
    /// 
    /// Source: AdvertisementForCategoryAmountOrderValidationRule
    /// 
    /// Внимание, в этой проверке как наследство erm есть две совершенно различные вещи, обозначаемые словом Category: рубрика и категория номенклатуры.
    /// </summary>
    public sealed class AdvertisementCountPerCategoryShouldBeLimitedActor
    {
        public const int MessageTypeId = 17;

        // Объявление в рубрике(Объявление под списком выдачи)
        private const int TargetCategoryCode = 38;

        private const int MaxPositionsPerCategory = 2;

        private readonly ValidationRuleShared _validationRuleShared;

        // todo: согласовать ошибку при всех типах проверок.
        // сейчас повторяется логика erm, но мне она кажется странной, ошибка должна быть на всех уровнях - нельзя пропускать лишний заказ в ядро.
        // например, аналогичная проверка на количество тематик в выпуске - выдает всегда ошибку.
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public AdvertisementCountPerCategoryShouldBeLimitedActor(ValidationRuleShared validationRuleShared)
        {
            _validationRuleShared = validationRuleShared;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            return _validationRuleShared.ProcessRule(GetValidationResults, MessageTypeId);
        }

        private static IQueryable<Version.ValidationResult> GetValidationResults(IQuery query, long version)
        {
            var data = query.For<Position>().Where(x => x.CategoryCode == TargetCategoryCode)
                .Join(query.For<OrderPosition>().Where(x => x.Category3Id.HasValue), x => x.Id, x => x.ItemPositionId, (position, orderPosition) => orderPosition)
                .Join(query.For<OrderPeriod>().Where(x => x.Scope == 0), x => x.OrderId, x => x.OrderId, (position, period) => new {position, period })
                .GroupBy(x => new { x.period.Start, x.period.OrganizationUnitId, x.position.Category1Id, x.position.Category3Id })
                .Select(x => new { x.Key.Start, x.Key.OrganizationUnitId, x.Key.Category1Id, x.Key.Category3Id, Count = x.Count() });

            var messages = from order in query.For<Order>()
                           from orderPosition in query.For<OrderPosition>().Where(x => x.OrderId == order.Id && x.ThemeId != null)
                           from orderPeriod in query.For<OrderPeriod>().Where(x => x.OrderId == order.Id)
                           from period in query.For<Period>().Where(x => x.Start == orderPeriod.Start && x.OrganizationUnitId == orderPeriod.OrganizationUnitId)
                           from category in query.For<Category>().Where(x => x.Id == (orderPosition.Category3Id ?? orderPosition.Category1Id))
                           from record in data.Where(x => x.OrganizationUnitId == orderPeriod.OrganizationUnitId && x.Start == orderPeriod.Start && x.Category1Id == orderPosition.Category1Id && x.Category3Id == orderPosition.Category3Id)
                           let count = orderPeriod.Scope == 0 ? record.Count : record.Count + 1
                           where count > MaxPositionsPerCategory
                           select new Version.ValidationResult
                               {
                                   MessageType = MessageTypeId,
                                   MessageParams =
                                       new XDocument(new XElement("root",
                                                                  new XElement("message",
                                                                               new XAttribute("max", TargetCategoryCode),
                                                                               new XAttribute("count", count),
                                                                               new XAttribute("name", category.Name),
                                                                               new XAttribute("month", orderPeriod.Start)),
                                                                  new XElement("order",
                                                                               new XAttribute("id", order.Id),
                                                                               new XAttribute("number", order.Number)))),
                                   PeriodStart = period.Start,
                                   PeriodEnd = period.End,
                                   ProjectId = period.ProjectId,
                                   VersionId = version,

                                   Result = RuleResult,
                               };

            return messages;
        }
    }
}
