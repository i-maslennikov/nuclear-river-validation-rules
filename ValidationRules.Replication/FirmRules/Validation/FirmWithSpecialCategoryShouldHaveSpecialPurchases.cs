using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;

using Firm = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates.Firm;
using Order = NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates.Order;
using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.FirmRules.Validation
{
    /// <summary>
    /// Для фирм, у которых есть рубрика N и нет продаж с категорией номенклатуры M, должна выводиться ошибка в массовом режиме, предупреждение - в единичном.
    /// "У фирмы {0}, с рубрикой "Выгодные покупки с 2ГИС", отсутствуют продажи по позициям "Самореклама только для ПК" или "Выгодные покупки с 2ГИС""
    /// 
    /// Source: AreThereAnyAdvertisementsInAdvantageousPurchasesRubricOrderValidationRule
    /// </summary>
    public sealed class FirmWithSpecialCategoryShouldHaveSpecialPurchases : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public FirmWithSpecialCategoryShouldHaveSpecialPurchases(IQuery query) : base(query, MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            // Заказы в локальном scope не участвуют в вычислении периодов:
            //  на другие заказы они не влияют
            //  а в них самих ошибка невозможна
            var publicOrders =
                from order in query.For<Order>().Where(x => x.Scope == 0)
                from orderPosition in query.For<Order.SpecialPosition>().Where(x => x.OrderId == order.Id)
                select order;

            var firstEmptyPeriods =
                from firm in query.For<Firm>().Where(x => x.NeedsSpecialPosition)
                let start = publicOrders.Where(x => x.FirmId == firm.Id).Min(x => (DateTime?)x.Begin) ?? DateTime.MaxValue
                select new { FirmId = firm.Id, Begin = DateTime.MinValue, End = start };

            var emptyPeriods =
                from firm in query.For<Firm>().Where(x => x.NeedsSpecialPosition)
                from order in publicOrders.Where(x => x.FirmId == firm.Id)
                from nextOverlappingOrder in publicOrders.Where(x => x.FirmId == firm.Id && x.Begin <= order.End && order.End < x.End).DefaultIfEmpty() // Заказ той же фирмы, продляющий наличие спецпозиции
                where nextOverlappingOrder == null
                let start = publicOrders.Where(x => x.FirmId == firm.Id && x.End > order.End).Min(x => (DateTime?)x.Begin) ?? DateTime.MaxValue // Заказ той же фирмы, не продляющий наличие спецпозиции, но возобновляющий её в будущем
                select new { order.FirmId, Begin = order.End, End = start };

            var results =
                from period in emptyPeriods.Union(firstEmptyPeriods)
                from order in query.For<Order>().Where(x => x.FirmId == period.FirmId)
                from firm in query.For<Firm>().Where(x => x.Id == period.FirmId)
                where !query.For<Order.SpecialPosition>().Any(x => x.OrderId == order.Id)
                where firm.NeedsSpecialPosition && order.Begin < period.End && order.End > period.Begin
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new XDocument(new XElement("root",
                                                       new XElement("firm",
                                                                    new XAttribute("id", firm.Id),
                                                                    new XAttribute("name", firm.Name)),
                                                       new XElement("order",
                                                                    new XAttribute("id", order.Id),
                                                                    new XAttribute("number", order.Number)))),
                        PeriodStart = period.Begin > order.Begin ? period.Begin : order.Begin,
                        PeriodEnd = period.End < order.End ? period.End : order.End,
                        ProjectId = order.ProjectId,

                        Result = RuleResult,
                    };

            return results;
        }
    }
}
