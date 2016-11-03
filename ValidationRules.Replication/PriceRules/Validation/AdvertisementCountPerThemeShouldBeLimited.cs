using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;
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

            var saleCounts =
                sales.GroupBy(x => new { x.Scope, x.Start, x.OrganizationUnitId, x.ThemeId })
                     .Select(x => new { x.Key, Count = x.Count() });

            var oversales =
                from sale in sales
                let count = saleCounts.Where(x => x.Key.ThemeId == sale.ThemeId
                                                  && x.Key.OrganizationUnitId == sale.OrganizationUnitId
                                                  && x.Key.Start == sale.Start
                                                  && Scope.CanSee(sale.Scope, x.Key.Scope))
                                      .Sum(x => x.Count)
                where count > MaxPositionsPerTheme
                select new { sale.Start, sale.OrganizationUnitId, sale.OrderId, sale.ThemeId, Count = count };

            var messages =
                from oversale in oversales
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
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return messages;
        }
    }
}
