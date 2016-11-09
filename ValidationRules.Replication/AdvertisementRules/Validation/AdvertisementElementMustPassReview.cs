using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, в которых есть невыверенные РМ, должна выводиться ошибка
    /// - В рекламном материале "{0}", который подлежит выверке, элемент "{1}" находится в статусе 'Черновик'
    /// 
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/OrdersCheckAdvertisementElementIsDraft
    /// 
    /// - В рекламном материале "{0}", который подлежит выверке, элемент "{1}" содержит ошибки выверки
    /// 
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/OrdersCheckAdvertisementElementWasInvalidated
    /// </summary>
    public sealed class AdvertisementElementMustPassReview : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public AdvertisementElementMustPassReview(IQuery query) : base(query, MessageTypeCode.AdvertisementElementMustPassReview)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from advertisementId in query.For<Order.OrderPositionAdvertisement>().Where(x => x.OrderId == order.Id).Select(x => x.AdvertisementId).Distinct()
                              from advertisement in query.For<Advertisement>().Where(x => x.Id == advertisementId)
                              from fail in query.For<Advertisement.ElementNotPassedReview>().Where(x => x.AdvertisementId == advertisement.Id)
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(
                                          new XElement("root",
                                              new XElement("order",
                                                  new XAttribute("id", order.Id),
                                                  new XAttribute("name", order.Number)),
                                              new XElement("advertisement",
                                                  new XAttribute("id", advertisement.Id),
                                                  new XAttribute("name", advertisement.Name)),
                                              new XElement("advertisementElement",
                                                  new XAttribute("id", fail.AdvertisementElementId),
                                                  new XAttribute("name", query.For<AdvertisementElementTemplate>().Single(x => x.Id == fail.AdvertisementElementTemplateId).Name)),
                                              new XElement("message",
                                                  new XAttribute("advertisementElementStatus", (int)fail.Status))
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
