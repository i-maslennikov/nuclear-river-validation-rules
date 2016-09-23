using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Если позиция заказа имеет РМ, в шаблоне которого в качестве заглушки выбран этот же РМ, то должна выводитьс ошибка:
    /// "Позиция {position} содержит заглушку рекламного материала"
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
                                                                                              new XAttribute("id", order.Id),
                                                                                              new XAttribute("number", order.Number)),
                                                                                  new XElement("position",
                                                                                              new XAttribute("id", fail.PositionId),
                                                                                              new XAttribute("name", query.For<Position>().Single(x => x.Id == fail.PositionId).Name))
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
