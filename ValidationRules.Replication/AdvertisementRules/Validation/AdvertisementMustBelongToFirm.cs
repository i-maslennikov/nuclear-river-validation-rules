using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Если для позиции заказа существует объект привязки с РМ, в РМ указана фирма и фирма заказа не содержит этот РМ, то должна выводиться ошибка:
    /// "В позиции {orderPosition} выбран рекламный материал {advertisement}, не принадлежащий фирме {firm}"
    /// 
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/AdvertisementSpecifiedForPositionDoesNotBelongToFirm
    /// </summary>
    public sealed class AdvertisementMustBelongToFirm : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public AdvertisementMustBelongToFirm(IQuery query) : base(query, MessageTypeCode.AdvertisementMustBelongToFirm)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              join fail in query.For<Order.AdvertisementMustBelongToFirm>() on order.Id equals fail.OrderId
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(new XElement("root",
                                                                                 new XElement("order",
                                                                                              new XAttribute("id", order.Id),
                                                                                              new XAttribute("number", order.Number)),
                                                                                  new XElement("orderPosition",
                                                                                              new XAttribute("id", fail.OrderPositionId),
                                                                                              new XAttribute("name", query.For<Position>().Single(x => x.Id == fail.PositionId).Name)),
                                                                                  new XElement("advertisement",
                                                                                              new XAttribute("id", fail.AdvertisementId),
                                                                                              new XAttribute("name", query.For<Advertisement>().Single(x => x.Id == fail.AdvertisementId).Name)),
                                                                                  new XElement("firm",
                                                                                              new XAttribute("id", fail.FirmId),
                                                                                              new XAttribute("name", query.For<Firm>().Single(x => x.Id == fail.FirmId).Name))
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
