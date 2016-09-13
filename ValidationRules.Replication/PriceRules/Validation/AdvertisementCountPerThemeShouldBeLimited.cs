using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказов и проектов, с превышением числа проданных тематик, должна выводиться ошибка.
    /// "Слишком много продаж в тематику {0}. Продано {1} позиций вместо {2} возможных"
    /// 
    /// Source: ThemePositionCountValidationRule
    /// </summary>
    public sealed class AdvertisementCountPerThemeShouldBeLimited : ValidationResultAccessorBase
    {
        private const int MaxPositionsPerTheme = 10;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public AdvertisementCountPerThemeShouldBeLimited(IQuery query) : base(query, MessageTypeCode.AdvertisementCountPerThemeShouldBeLimited)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var sales =
                from orderPosition in query.For<OrderPosition>().Where(x => x.ThemeId != null)
                from orderPeriod in query.For<OrderPeriod>().Where(x => x.OrderId == orderPosition.OrderId)
                select new { orderPosition.OrderId, orderPeriod.Scope, orderPeriod.Start, orderPeriod.OrganizationUnitId, orderPosition.ThemeId };

            var approvedSaleCounts =
                sales.Where(x => x.Scope == 0)
                         .GroupBy(x => new { x.Start, x.OrganizationUnitId, x.ThemeId })
                         .Select(x => new { x.Key, Count = x.Count() });

            var perOrderSaleCounts =
                sales.GroupBy(x => new { x.OrderId, x.Scope, x.Start, x.OrganizationUnitId, x.ThemeId })
                         .Select(x => new { x.Key, Count = x.Count() });

            var oversales =
                from c in approvedSaleCounts
                join co in perOrderSaleCounts on c.Key equals new { co.Key.Start, co.Key.OrganizationUnitId, co.Key.ThemeId }
                let count = c.Count + (co.Key.Scope == 0 ? 0 : co.Count)
                where count > MaxPositionsPerTheme
                select new { c.Key.Start, c.Key.OrganizationUnitId, co.Key.OrderId, Count = count, c.Key.ThemeId };

            var messages =
                from oversale in oversales.Distinct()
                join period in query.For<Period>() on new { oversale.Start, oversale.OrganizationUnitId } equals new { period.Start, period.OrganizationUnitId }
                join order in query.For<Order>() on oversale.OrderId equals order.Id
                join theme in query.For<Theme>() on oversale.ThemeId equals theme.Id
                select new Version.ValidationResult
                {
                                   MessageParams =
                                       new XDocument(new XElement("root",
                                                                  new XElement("message",
                                                                               new XAttribute("max", MaxPositionsPerTheme),
                                                                               new XAttribute("count", oversale.Count)),
                                                                  new XElement("theme",
                                                                               new XAttribute("id", theme.Id),
                                                                               new XAttribute("name", theme.Name)),
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
