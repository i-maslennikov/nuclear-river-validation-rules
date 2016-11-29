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
    /// Для фирм, у которых есть рубрика N и нет продаж с категорией номенклатуры M, должна выводиться ошибка в массовом режиме, предупреждение - в единичном.
    /// "У фирмы {0}, с рубрикой "Выгодные покупки с 2ГИС", отсутствуют продажи по позициям "Самореклама только для ПК" или "Выгодные покупки с 2ГИС""
    /// 
    /// Source: AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule
    /// </summary>
    public sealed class FirmWithSpecialCategoryShouldHaveSpecialPurchasesOrder : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.None)
                                                                    .WhenMassRelease(Result.None);

        public FirmWithSpecialCategoryShouldHaveSpecialPurchasesOrder(IQuery query) : base(query, MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchasesOrder)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var dates =
                query.For<Firm.AdvantageousPurchasePositionDistributionPeriod>().Select(x => new { x.FirmId, Date = x.Begin })
                     .Union(query.For<Firm.AdvantageousPurchasePositionDistributionPeriod>().Select(x => new { x.FirmId, Date = x.End }));

            var results =
                from begin in dates
                from end in dates.Where(x => x.FirmId == begin.FirmId && x.Date > begin.Date).OrderBy(x => x.Date).Take(1)
                from firm in query.For<Firm>().Where(x => x.Id == begin.FirmId)
                from order in query.For<Order>().Where(x => x.FirmId == begin.FirmId && x.Begin < end.Date && begin.Date < x.End)
                where query.For<Firm.AdvantageousPurchasePositionDistributionPeriod>()
                           .Where(x => x.FirmId == begin.FirmId && x.Begin < end.Date && begin.Date < x.End && Scope.CanSee(order.Scope, x.Scope))
                           .All(x => !x.HasPosition)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new XDocument(new XElement("root",
                                new XElement("firm",
                                    new XAttribute("id", firm.Id),
                                    new XAttribute("name", firm.Name)),
                                new XElement("order",
                                    new XAttribute("id", order.Id),
                                    new XAttribute("name", order.Number)))),

                        PeriodStart = begin.Date,
                        PeriodEnd = end.Date,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return results;
        }
    }
}
