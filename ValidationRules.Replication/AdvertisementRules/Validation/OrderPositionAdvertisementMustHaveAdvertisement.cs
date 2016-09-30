using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, у которых хотя бы один шаблон РМ обязательный, и для этого шаблона не указан РМ, должна выводиться ошибка:
    /// "В позиции {0} необходимо указать рекламные материалы"
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/OrderCheckPositionMustHaveAdvertisements
    /// 
    /// "В позиции {0} необходимо указать рекламные материалы для подпозиции '{1}'"
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/OrderCheckCompositePositionMustHaveLinkingObject
    /// </summary>
    public sealed class OrderPositionAdvertisementMustHaveAdvertisement : ValidationResultAccessorBase
    {
        private static readonly int LocalRuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                         .WhenMass(Result.Error)
                                                                         .WhenMassPrerelease(Result.Error)
                                                                         .WhenMassRelease(Result.Error);

        // Наличие двух кодов в одной проверке закрывает путь к отделению этого кода от логики проверки
        // поэтому нужно быть готовым к тому, что проверка будет разделена на две, если не будет найдено другого решения.
        private static readonly int RegionalRuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                            .WhenMass(Result.Error)
                                                                            .WhenMassPrerelease(Result.Error)
                                                                            .WhenMassRelease(Result.Error);

        public OrderPositionAdvertisementMustHaveAdvertisement(IQuery query) : base(query, MessageTypeCode.OrderPositionAdvertisementMustHaveAdvertisement)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              join fail in query.For<Order.MissingAdvertisementReference>() on order.Id equals fail.OrderId
                              let isRegional = query.For<Order.LinkedProject>().Count(x => x.OrderId == order.Id) > 1
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(new XElement("root",
                                                                                 new XElement("order",
                                                                                              new XAttribute("id", order.Id),
                                                                                              new XAttribute("number", order.Number)),
                                                                                 new XElement("orderPosition",
                                                                                              new XAttribute("id", fail.OrderPositionId),
                                                                                              new XAttribute("name", query.For<Position>().Single(x => x.Id == fail.CompositePositionId).Name)),
                                                                                 new XElement("position",
                                                                                              new XAttribute("id", fail.PositionId),
                                                                                              new XAttribute("name", query.For<Position>().Single(x => x.Id == fail.PositionId).Name))
                                                                     )),
                                      PeriodStart = order.BeginDistributionDate,
                                      PeriodEnd = order.EndDistributionDatePlan,
                                      ProjectId = order.ProjectId,

                                      Result = isRegional ? RegionalRuleResult : LocalRuleResult,
                                  };

            return ruleResults;
        }
    }
}
