using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

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
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
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
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(new XElement("root",
                                          new XElement("order",
                                            new XAttribute("id", order.Id)),
                                          new XElement("orderPosition",
                                            new XAttribute("id", fail.OrderPositionId),
                                            new XElement("position", new XAttribute("id", fail.CompositePositionId))),
                                          new XElement("opa",
                                            new XElement("orderPosition",new XAttribute("id", fail.OrderPositionId)),
                                            new XElement("position", new XAttribute("id", fail.PositionId)))
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
