using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, с незаполненными обязательными для заполнения ЭРМ, дожна выводиться ошибка
    /// "В рекламном материале {0} не заполнен обязательный элемент {1}"
    /// 
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/OrdersCheckPositionMustHaveAdvertisementElements
    /// </summary>
    public sealed class OrderMustHaveAdvertisement : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public OrderMustHaveAdvertisement(IQuery query) : base(query, MessageTypeCode.OrderMustHaveAdvertisement)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from advertisementId in query.For<Order.OrderPositionAdvertisement>().Where(x => x.OrderId == order.Id).Select(x => x.AdvertisementId).Distinct()
                              from advertisement in query.For<Advertisement>().Where(x => x.Id == advertisementId)
                              join fail in query.For<Advertisement.RequiredElementMissing>() on advertisement.Id equals fail.AdvertisementId
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(new XElement("root",
                                          new XElement("order",
                                              new XAttribute("id", order.Id)),
                                          new XElement("advertisement",
                                              new XAttribute("id", advertisement.Id)),
                                          new XElement("advertisementElement",
                                              new XAttribute("id", fail.AdvertisementElementId),
                                              new XElement("advertisementElementTemplate",
                                              new XAttribute("id", fail.AdvertisementElementTemplateId)))
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
