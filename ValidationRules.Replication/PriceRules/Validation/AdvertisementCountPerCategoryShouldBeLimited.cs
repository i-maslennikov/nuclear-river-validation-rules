using System.Linq;
using System.Xml.Linq;

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
    public sealed class AdvertisementCountPerCategoryShouldBeLimited : ValidationResultAccessorBase
    {
        // Объявление в рубрике(Объявление под списком выдачи)
        private const int TargetCategoryCode = 38;

        private const int MaxPositionsPerCategory = 2;

        // todo: согласовать ошибку при всех типах проверок.
        // сейчас повторяется логика erm, но мне она кажется странной, ошибка должна быть на всех уровнях - нельзя пропускать лишний заказ в ядро.
        // например, аналогичная проверка на количество тематик в выпуске - выдает всегда ошибку.
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public AdvertisementCountPerCategoryShouldBeLimited(IQuery query) : base(query, MessageTypeCode.AdvertisementCountPerCategoryShouldBeLimited)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var sales =
                from orderPosition in query.For<OrderPosition>().Where(x => x.Category3Id != null || x.Category1Id != null)
                from orderPeriod in query.For<OrderPeriod>().Where(x => x.OrderId == orderPosition.OrderId)
                from position in query.For<Position>().Where(x => x.Id == orderPosition.ItemPositionId && x.CategoryCode == TargetCategoryCode)
                select new { orderPosition.OrderId, orderPeriod.Scope, orderPeriod.Start, orderPeriod.OrganizationUnitId, orderPosition.Category1Id, orderPosition.Category3Id };

            var approvedSaleCounts =
                sales.Where(x => x.Scope == 0)
                         .GroupBy(x => new { x.Start, x.OrganizationUnitId, x.Category1Id, x.Category3Id })
                         .Select(x => new { x.Key, Count = x.Count() });

            var perOrderSaleCounts =
                sales.GroupBy(x => new { x.OrderId, x.Scope, x.Start, x.OrganizationUnitId, x.Category1Id, x.Category3Id })
                         .Select(x => new { x.Key, Count = x.Count() });

            var oversales =
                from c in approvedSaleCounts
                join co in perOrderSaleCounts on c.Key equals new { co.Key.Start, co.Key.OrganizationUnitId, co.Key.Category1Id, co.Key.Category3Id }
                let count = c.Count + (co.Key.Scope == 0 ? 0 : co.Count)
                where count > MaxPositionsPerCategory
                select new { c.Key.Start, c.Key.OrganizationUnitId, co.Key.OrderId, Count = count, c.Key.Category1Id, c.Key.Category3Id };

            var messages =
                from oversale in oversales.Distinct()
                join period in query.For<Period>() on new { oversale.Start , oversale.OrganizationUnitId }  equals new { period.Start, period.OrganizationUnitId}
                join order in query.For<Order>() on oversale.OrderId equals order.Id
                join category in query.For<Category>() on oversale.Category3Id ?? oversale.Category1Id equals category.Id
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new XDocument(new XElement("root",
                                                       new XElement("message",
                                                                    new XAttribute("max", MaxPositionsPerCategory),
                                                                    new XAttribute("count", oversale.Count),
                                                                    new XAttribute("name", category.Name),
                                                                    new XAttribute("month", oversale.Start)),
                                                       new XElement("order",
                                                                    new XAttribute("id", order.Id),
                                                                    new XAttribute("number", order.Number)))),
                        PeriodStart = period.Start,
                        PeriodEnd = period.End,
                        ProjectId = period.ProjectId,

                        Result = RuleResult,
                    };


            return messages;
        }
    }
}
