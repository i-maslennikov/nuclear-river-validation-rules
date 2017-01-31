using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, РМ которых являются заглушками, должна выводиться ошибка
    /// "Позиция {0} содержит заглушку рекламного материала"
    /// 
    /// Source: DummyAdvertisementOrderValidationRule/OrderContainsDummyAdvertisementError
    /// </summary>
    public sealed class OrderMustNotContainDummyAdvertisement : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Warning)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public OrderMustNotContainDummyAdvertisement(IQuery query) : base(query, MessageTypeCode.OrderMustNotContainDummyAdvertisement)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              join fail in query.For<Order.AdvertisementIsDummy>() on order.Id equals fail.OrderId
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(new XElement("root",
                                          new XElement("order",
                                              new XAttribute("id", order.Id)),
                                          new XElement("opa",
                                              new XElement("orderPosition", new XAttribute("id", fail.OrderPositionId)),
                                              new XElement("position",new XAttribute("id", fail.PositionId)))
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
