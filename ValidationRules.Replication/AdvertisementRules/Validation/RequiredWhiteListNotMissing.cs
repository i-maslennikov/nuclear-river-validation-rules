using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

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
    public sealed class RequiredWhiteListNotMissing : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Info)
                                                                    .WhenMass(Result.Info)
                                                                    .WhenMassPrerelease(Result.Info)
                                                                    .WhenMassRelease(Result.Info);

        public RequiredWhiteListNotMissing(IQuery query) : base(query, MessageTypeCode.RequiredWhiteListNotMissing)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              join fail in query.For<Order.WhiteListAdvertisement>() on order.Id equals fail.OrderId
                              where fail.AdvertisementId != null
                              select new Version.ValidationResult
                                  {
                                  MessageParams = new XDocument(new XElement("root",
                                                                                 new XElement("order",
                                                                                              new XAttribute("id", order.Id),
                                                                                              new XAttribute("number", order.Number)),
                                                                                  new XElement("firm",
                                                                                              new XAttribute("id", fail.FirmId),
                                                                                              new XAttribute("name", query.For<Firm>().Single(x => x.Id == fail.FirmId).Name)),
                                                                                  new XElement("advertisement",
                                                                                              new XAttribute("id", fail.AdvertisementId),
                                                                                              new XAttribute("name", query.For<Advertisement>().Single(x => x.Id == fail.AdvertisementId.Value).Name))
                                                                                  )),
                                      PeriodStart = order.BeginDistributionDate,
                                      PeriodEnd = order.EndDistributionDatePlan,
                                      ProjectId = order.ProjectId,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
