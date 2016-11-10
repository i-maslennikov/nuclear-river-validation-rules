using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для проекта, в котором продано недостаточно рекламы в Position.Category должна выводиться ошибка.
    /// "Позиция {0} должна присутствовать в сборке в количестве от {1} до {2}. Фактическое количество позиций в месяц {6} - {3} (оформлено - {4}, содержит ошибки - {5})"
    /// В силу того, что мы (пока) не знаем, число заказов с ошибками, сократим сообщение (erm тоже так делает иногда)
    /// "Позиция {0} должна присутствовать в сборке в количестве от {1} до {2}. Фактическое количество позиций в месяц {3} - {4}"
    /// 
    /// Source: AdvertisementAmountOrderValidationRule/AdvertisementAmountErrorMessage
    /// </summary>
    public sealed class MinimumAdvertisementAmount : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public MinimumAdvertisementAmount(IQuery query) : base(query, MessageTypeCode.MinimumAdvertisementAmount)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var restrictionGrid =
                from restriction in query.For<AdvertisementAmountRestriction>()
                from pp in query.For<PricePeriod>().Where(x => x.PriceId == restriction.PriceId)
                select new { pp.Start, pp.OrganizationUnitId, restriction.CategoryCode, restriction.Min, restriction.Max, restriction.CategoryName };

            var saleGrid =
                from position in query.For<AmountControlledPosition>()
                from op in query.For<OrderPeriod>().Where(x => x.OrderId == position.OrderId)
                group new { op.Start, op.OrganizationUnitId, position.CategoryCode, op.Scope }
                    by new { op.Start, op.OrganizationUnitId, position.CategoryCode, op.Scope }
                into groups
                select new { groups.Key, Count = groups.Count() };

            var violations =
                from position in query.For<AmountControlledPosition>()
                from op in query.For<OrderPeriod>().Where(x => x.OrderId == position.OrderId)
                from restriction in restrictionGrid.Where(x => x.Start == op.Start &&
                                                               x.OrganizationUnitId == op.OrganizationUnitId &&
                                                               x.CategoryCode == position.CategoryCode)
                let count = saleGrid.Where(x => x.Key.CategoryCode == position.CategoryCode &&
                                                x.Key.OrganizationUnitId == op.OrganizationUnitId &&
                                                x.Key.Start == op.Start &&
                                                Scope.CanSee(op.Scope, x.Key.Scope))
                                    .Sum(x => x.Count)
                where count < restriction.Min
                select new { op.OrderId, op.Start, op.OrganizationUnitId, restriction.Min, restriction.Max, Count = count, restriction.CategoryName };

            var messages =
                from violation in violations
                from order in query.For<Order>().Where(x => x.Id == violation.OrderId)
                from period in query.For<Period>().Where(x => x.Start == violation.Start && x.OrganizationUnitId == violation.OrganizationUnitId)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new XDocument(new XElement("root",
                                new XElement("message",
                                    new XAttribute("min", violation.Min),
                                    new XAttribute("max", violation.Max),
                                    new XAttribute("count", violation.Count),
                                    new XAttribute("name", violation.CategoryName),
                                    new XAttribute("month", violation.Start)),
                                new XElement("order",
                                    new XAttribute("id", order.Id),
                                    new XAttribute("name", order.Number)))),

                        PeriodStart = period.Start,
                        PeriodEnd = period.End,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return messages;
        }
    }
}
