using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

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
                              join relation in query.For<Order.OrderAdvertisement>() on order.Id equals relation.OrderId
                              join advertisement in query.For<Advertisement>() on relation.AdvertisementId equals advertisement.Id
                              join fail in query.For<Advertisement.ElementNotPassedReview>() on advertisement.Id equals fail.AdvertisementId
                              select new Version.ValidationResult
                                  {
                                  MessageParams = new XDocument(new XElement("root",
                                                                                 new XElement("order",
                                                                                              new XAttribute("id", order.Id),
                                                                                              new XAttribute("number", order.Number)),
                                                                                  new XElement("advertisement",
                                                                                              new XAttribute("id", advertisement.Id),
                                                                                              new XAttribute("name", advertisement.Name)),
                                                                                  new XElement("advertisementElement",
                                                                                              new XAttribute("id", fail.AdvertisementElementId),
                                                                                              new XAttribute("name", query.For<AdvertisementElementTemplate>().Single(x => x.Id == fail.AdvertisementElementTemplateId).Name)),
                                                                                  new XElement("advertisementElementStatus",
                                                                                              new XAttribute("id", fail.Status))
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
