using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, у которых текст рекламного материала повторяет контактную ссылку фирмы, должно выводиться предупреждение
    /// 
    /// Для фирмы {0} заказана рекламная ссылка {1} позиция {2}, дублирующая контакт фирмы
    /// 
    /// Source: ContactDoesntContainSponsoredLinkOrderValidationRule/FirmContactContainsSponsoredLinkError
    /// </summary>
    public sealed class AdvertisementWebsiteShouldNotBeFirmWebsite : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.None)
                                                                    .WhenMassRelease(Result.None);

        public AdvertisementWebsiteShouldNotBeFirmWebsite(IQuery query) : base(query, MessageTypeCode.AdvertisementWebsiteShouldNotBeFirmWebsite)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from firmWebSite in query.For<Firm.FirmWebsite>().Where(x => x.FirmId == order.FirmId)
                              from opa in query.For<Order.OrderPositionAdvertisement>().Where(x => x.OrderId == order.Id)
                              from advertisementWebsite in query.For<Advertisement.AdvertisementWebsite>().Where(x => x.AdvertisementId == opa.AdvertisementId)
                              where advertisementWebsite.Website.StartsWith(firmWebSite.Website) // рекламная ссылка начинается также как контактная
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(new XElement("root",
                                          new XElement("order",
                                              new XAttribute("id", order.Id),
                                              new XAttribute("name", order.Number)),
                                          new XElement("firm",
                                              new XAttribute("id", order.FirmId),
                                              new XAttribute("name", query.For<Firm>().Single(x => x.Id == order.FirmId).Name)),
                                          new XElement("orderPosition",
                                              new XAttribute("id", opa.OrderPositionId),
                                              new XAttribute("name", query.For<Position>().Single(x => x.Id == opa.PositionId).Name)),
                                          new XElement("message",
                                              new XAttribute("website", firmWebSite.Website))
                                          )),
                                      PeriodStart = order.BeginDistributionDate,
                                      PeriodEnd = order.EndDistributionDatePlan,
                                      OrderId = order.Id,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
