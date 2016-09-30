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
            var categories =
                from order in query.For<Order>()
                from categoryPurchase in query.For<Order.CategoryPurchase>().Where(x => x.OrderId == order.Id)
                select new { order.FirmId, BeginDistributionDate = order.Begin, EndDistributionDate = order.End, order.Scope, categoryPurchase.CategoryId };

            var messages =
                from order in query.For<Order>()
                from firm in query.For<Firm>().Where(x => x.Id == order.FirmId)
                let c = categories.Where(x => x.FirmId == order.FirmId
                                              && x.BeginDistributionDate <= order.End
                                              && x.EndDistributionDate >= order.Begin
                                              && (x.Scope == 0 || x.Scope == order.Scope)).Select(x => x.CategoryId).Distinct()
                where c.Count() > MaxCategoriesAlowedForFirm
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new XDocument(new XElement("root",
                                                       new XElement("message",
                                                                    new XAttribute("count", c.Count()),
                                                                    new XAttribute("allowed", MaxCategoriesAlowedForFirm)),
                                                       new XElement("firm",
                                                                    new XAttribute("id", firm.Id),
                                                                    new XAttribute("name", firm.Name)),
                                                       new XElement("order",
                                                                    new XAttribute("id", order.Id),
                                                                    new XAttribute("number", order.Number)))),
                        PeriodStart = order.Begin,
                        PeriodEnd = order.End,
                        ProjectId = order.ProjectId,

                        Result = RuleResult,
                    };


            return messages;
        }
    }
}
