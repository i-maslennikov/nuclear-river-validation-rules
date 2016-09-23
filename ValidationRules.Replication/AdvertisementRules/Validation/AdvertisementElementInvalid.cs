using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Если для позиции заказа существует РМ, такой что существует ЭРМ с шаблоном, требующим выверки, и ЭРМ имеет статус 'Выверено с ошибками', , то должна выводиться ошибка:
    /// "В рекламном материале {advertisement}, который подлежит выверке, элемент {advertisementElement} содержит ошибки выверки"
    /// 
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/OrdersCheckAdvertisementElementWasInvalidated
    /// </summary>
    public sealed class AdvertisementElementInvalid : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public AdvertisementElementInvalid(IQuery query) : base(query, MessageTypeCode.AdvertisementElementInvalid)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              join relation in query.For<Order.OrderAdvertisement>() on order.Id equals relation.OrderId
                              join advertisement in query.For<Advertisement>() on relation.AdvertisementId equals advertisement.Id
                              join fail in query.For<Advertisement.ElementInvalid>() on advertisement.Id equals fail.AdvertisementId
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
                                                                                              new XAttribute("name", query.For<AdvertisementElementTemplate>().Single(x => x.Id == fail.AdvertisementElementTemplateId).Name))
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
