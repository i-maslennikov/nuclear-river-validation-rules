using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для заказов, которые приводят к превышению ограничения на максимальное количесов рекламы в Position.Category должна выводиться ошибка.
    /// "Позиция {0} должна присутствовать в сборке в количестве от {1} до {2}. Фактическое количество позиций в месяц {6} - {3} (оформлено - {4}, содержит ошибки - {5})"
    /// В силу того, что мы (пока) не знаем, число заказов с ошибками, сократим сообщение (erm тоже так делает иногда)
    /// "Позиция {0} должна присутствовать в сборке в количестве от {1} до {2}. Фактическое количество позиций в месяц {3} - {4}"
    /// 
    /// Source: AdvertisementAmountOrderValidationRule/AdvertisementAmountErrorMessage
    /// </summary>
    public sealed class MaximumAdvertisementAmount : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public MaximumAdvertisementAmount(IQuery query) : base(query, MessageTypeCode.MaximumAdvertisementAmount)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var restrictionGrid = from restriction in query.For<AdvertisementAmountRestriction>()
                                  join pp in query.For<PricePeriod>() on restriction.PriceId equals pp.PriceId
                                  select new { Key = new { pp.Start, pp.OrganizationUnitId, restriction.CategoryCode }, restriction.Min, restriction.Max, restriction.CategoryName };

            var saleGrid = from position in query.For<AmountControlledPosition>()
                           join op in query.For<OrderPeriod>() on position.OrderId equals op.OrderId
                           where op.Scope == 0
                           group new { op.Start, op.OrganizationUnitId, position.CategoryCode, op.Scope }
                               by new { op.Start, op.OrganizationUnitId, position.CategoryCode } into groups
                           select new { groups.Key, Count = groups.Count() };

            var ruleViolations = from restriction in restrictionGrid
                                 from sale in saleGrid.Where(x => x.Key == restriction.Key).DefaultIfEmpty()
                                 select new { restriction.Key, restriction.Min, restriction.Max, sale.Count, restriction.CategoryName };

            var orderRuleViolations = from position in query.For<AmountControlledPosition>()
                                      join op in query.For<OrderPeriod>() on position.OrderId equals op.OrderId
                                      join order in query.For<Order>() on op.OrderId equals order.Id
                                      join violation in ruleViolations on new { op.Start, op.OrganizationUnitId, position.CategoryCode }
                                          equals new { violation.Key.Start, violation.Key.OrganizationUnitId, violation.Key.CategoryCode }
                                      join period in query.For<Period>() on new { op.Start, op.OrganizationUnitId }
                                          equals new { period.Start, period.OrganizationUnitId }
                                      join project in query.For<Project>() on period.ProjectId equals project.Id
                                      let count = op.Scope == 0 ? violation.Count : violation.Count + 1
                                      where count > violation.Max
                                      select new Version.ValidationResult
                                      {
                                          MessageParams =
                                                new XDocument(new XElement("root",
                                                                new XElement("message",
                                                                            new XAttribute("min", violation.Min),
                                                                            new XAttribute("max", violation.Max),
                                                                            new XAttribute("count", count),
                                                                            new XAttribute("name", violation.CategoryName),
                                                                            new XAttribute("month", period.Start)),
                                                                new XElement("order",
                                                                            new XAttribute("id", order.Id),
                                                                            new XAttribute("number", order.Number)),
                                                                new XElement("project",
                                                                            new XAttribute("id", project.Id),
                                                                            new XAttribute("number", project.Name)))),
                                          PeriodStart = period.Start,
                                          PeriodEnd = period.End,
                                          ProjectId = period.ProjectId,

                                          Result = RuleResult,
                                      };

            return orderRuleViolations;
        }
    }
}
