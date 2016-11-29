using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.FirmRules.Validation
{
    /// <summary>
    /// Для фирм, которые одновременно размещаются более чем в N рубриках, должно выводиться предупреждение.
    /// "Для фирмы {0} задано слишком большое число рубрик - {1}. Максимально допустимое - {2}"
    /// 
    /// Source: CategoriesForFirmAmountOrderValidationRule
    /// </summary>
    public sealed class FirmShouldHaveLimitedCategoryCount : ValidationResultAccessorBase
    {
        private const int MaxCategoriesAlowedForFirm = 20;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Warning)
                                                                    .WhenMassPrerelease(Result.Warning)
                                                                    .WhenMassRelease(Result.None);

        public FirmShouldHaveLimitedCategoryCount(IQuery query) : base(query, MessageTypeCode.FirmShouldHaveLimitedCategoryCount)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var dates = query.For<Order>().Select(x => new { Date = x.Begin, x.FirmId })
                             .Union(query.For<Order>().Select(x => new { Date = x.End, x.FirmId }));

            var firmPeriods =
                from date in dates
                from nextDate in dates.OrderBy(x => x.Date).Where(x => x.FirmId == date.FirmId && x.Date > date.Date).Take(1)
                select new { date.FirmId, Begin = date.Date, End = nextDate.Date };

            var categoryPurchases =
                from order in query.For<Order>()
                from purchase in query.For<Order.CategoryPurchase>().Where(x => x.OrderId == order.Id)
                select new { order.Begin, order.End, order.Scope, purchase.CategoryId, order.FirmId };

            var messages =
                from firmPeriod in firmPeriods
                from order in query.For<Order>().Where(x => x.FirmId == firmPeriod.FirmId && x.Begin <= firmPeriod.Begin && firmPeriod.End <= x.End)
                let count = categoryPurchases.Where(x => x.FirmId == firmPeriod.FirmId && x.Begin <= firmPeriod.Begin && firmPeriod.End <= x.End && Scope.CanSee(order.Scope, x.Scope))
                                             .Select(x => x.CategoryId)
                                             .Distinct()
                                             .Count()
                where count > MaxCategoriesAlowedForFirm
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new XDocument(new XElement("root",
                                new XElement("message",
                                    new XAttribute("count", count),
                                    new XAttribute("allowed", MaxCategoriesAlowedForFirm)),
                                new XElement("firm",
                                    new XAttribute("id", firmPeriod.FirmId),
                                    new XAttribute("name", query.For<Firm>().Single(x => x.Id == firmPeriod.FirmId).Name)),
                                new XElement("order",
                                    new XAttribute("id", order.Id),
                                    new XAttribute("name", order.Number)))),

                        PeriodStart = firmPeriod.Begin,
                        PeriodEnd = firmPeriod.End,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return messages;
        }
    }
}
