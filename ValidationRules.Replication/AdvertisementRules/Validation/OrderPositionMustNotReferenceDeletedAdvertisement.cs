using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказаов, у которых есть удалённые РМ, должна выводиться ошибка
    /// В позиции {0} выбран удалённый рекламный материал {1}
    /// 
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/RemovedAdvertisemendSpecifiedForPosition
    /// </summary>
    public sealed class OrderPositionMustNotReferenceDeletedAdvertisement : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public OrderPositionMustNotReferenceDeletedAdvertisement(IQuery query) : base(query, MessageTypeCode.OrderPositionMustNotReferenceDeletedAdvertisement)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              join fail in query.For<Order.AdvertisementDeleted>() on order.Id equals fail.OrderId
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(new XElement("root",
                                          new XElement("order",
                                              new XAttribute("id", order.Id)),
                                          new XElement("opa",
                                              new XElement("orderPosition", new XAttribute("id", fail.OrderPositionId)),
                                              new XElement("position", new XAttribute("id", fail.PositionId))),
                                          new XElement("advertisement",
                                              new XAttribute("id", fail.AdvertisementId))
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
