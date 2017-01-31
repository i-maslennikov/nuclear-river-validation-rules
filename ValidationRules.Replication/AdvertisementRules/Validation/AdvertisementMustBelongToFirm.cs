using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, в РМ которых указана фирма, не являющейся фирмой заказа, должна выводиться ошибка
    /// -В позиции {0} выбран рекламный материал {1}, не принадлежащий фирме {2}
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
                                      MessageParams = new XDocument(
                                          new XElement("root",
                                              new XElement("order",
                                                  new XAttribute("id", order.Id)),
                                              new XElement("opa",
                                                  new XElement("orderPosition", new XAttribute("id", fail.OrderPositionId)),
                                                  new XElement("position",new XAttribute("id", fail.PositionId))),
                                              new XElement("advertisement",
                                                  new XAttribute("id", fail.AdvertisementId)),
                                              new XElement("firm",
                                                  new XAttribute("id", fail.FirmId)))),

                                      PeriodStart = order.BeginDistributionDate,
                                      PeriodEnd = order.EndDistributionDatePlan,
                                      OrderId = order.Id,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
