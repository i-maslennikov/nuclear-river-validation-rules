using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для фирм, у которых выбран в белый список РМ, должно выводиться информационное сообщение
    /// Для фирмы {0} в белый список выбран рекламный материал {1}
    /// 
    /// Source: AdvertisementsOnlyWhiteListOrderValidationRule/AdvertisementChoosenForWhitelist
    /// 
    /// * Поскольку проверок фирм нет, то сообщения выводим в заказах этой фирмы, в которых есть как минимум один РМ с возможностью выбора в белый список.
    /// </summary>
    public sealed class WhiteListAdvertisementMayPresent : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Info)
                                                                    .WhenMass(Result.Info)
                                                                    .WhenMassPrerelease(Result.Info)
                                                                    .WhenMassRelease(Result.Info);

        public WhiteListAdvertisementMayPresent(IQuery query) : base(query, MessageTypeCode.WhiteListAdvertisementMayPresent)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>().Where(x => x.RequireWhiteListAdvertisement)
                from advertisement in query.For<Advertisement>().Where(x => x.FirmId == order.FirmId && x.IsSelectedToWhiteList)
                from period in query.For<Firm.WhiteListDistributionPeriod>()
                                    .Where(x => x.FirmId == order.FirmId && x.Start < order.EndDistributionDatePlan && order.BeginDistributionDate < x.End)
                                    .Where(x => x.ProvidedByOrderId.HasValue).DefaultIfEmpty()
                where period != null || order.ProvideWhiteListAdvertisement
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(
                            new XElement("root",
                                new XElement("order",
                                    new XAttribute("id", order.Id),
                                    new XAttribute("name", order.Number)),
                                new XElement("firm",
                                    new XAttribute("id", order.FirmId),
                                    new XAttribute("name", query.For<Firm>().Single(x => x.Id == order.FirmId).Name)),
                                new XElement("advertisement",
                                    new XAttribute("id", advertisement.Id),
                                    new XAttribute("name", advertisement.Name)))),
                        PeriodStart = period != null && period.Start > order.BeginDistributionDate ? period.Start : order.BeginDistributionDate,
                        PeriodEnd = period != null && period.End < order.EndDistributionDatePlan ? period.End : order.EndDistributionDatePlan,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return ruleResults;
        }
    }
}
