using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;

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
            dates = dates.Select(x => new { x.Date, x.FirmId });

            var firmPeriods =
                from date in dates
                let nextDate = dates.Where(x => x.FirmId == date.FirmId && x.Date > date.Date).Min(x => (DateTime?)x.Date)
                where nextDate.HasValue
                select new { date.FirmId, Start = date.Date, End = nextDate.Value };

            var dtos =
                from order in query.For<Order>()
                from firmPeriod in firmPeriods.Where(x => x.FirmId == order.FirmId &&
                                                          x.Start >= order.Begin &&
                                                          x.End <= order.End)
                select new
                    {
                        order.ProjectId,
                        order.FirmId,
                        OrderId = order.Id,
                        OrderNumber = order.Number,
                        firmPeriod.Start,
                        firmPeriod.End,
                        order.Scope,
                    };

            var messages =
                from dto in dtos
                let categoryCount = query.For<Order.CategoryPurchase>().Where(x => x.OrderId == dto.OrderId).Select(x => x.CategoryId)
                              .Union(from dto2 in dtos.Where(x => x.FirmId == dto.FirmId &&
                                                            x.Start == dto.Start &&
                                                            x.End == dto.End &&
                                                            (x.Scope == 0 || x.Scope == dto.Scope))
                                     from categoryPurchase in query.For<Order.CategoryPurchase>().Where(x => x.OrderId == dto2.OrderId)
                                     select categoryPurchase.CategoryId
                                    ).Count()
                where categoryCount > MaxCategoriesAlowedForFirm
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new XDocument(new XElement("root",
                                                       new XElement("message",
                                                                    new XAttribute("count", categoryCount),
                                                                    new XAttribute("allowed", MaxCategoriesAlowedForFirm)),
                                                       new XElement("firm",
                                                                    new XAttribute("id", dto.FirmId),
                                                                    new XAttribute("name", query.For<Firm>().Single(x => x.Id == dto.FirmId).Name)),
                                                       new XElement("order",
                                                                    new XAttribute("id", dto.OrderId),
                                                                    new XAttribute("number", dto.OrderNumber))
                                        )),
                        PeriodStart = dto.Start,
                        PeriodEnd = dto.End,
                        ProjectId = dto.ProjectId,

                        Result = RuleResult,
                    };


            return messages;
        }
    }
}
