using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, у которых есть сложная позиция с обязательным составом и не указан объект привязки для любой подпозиции, должна выводиться ошибка
    /// В позиции {0} необходимо указать хотя бы один объект привязки для подпозиции {0}
    /// 
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/OrderCheckCompositePositionMustHaveLinkingObject
    /// </summary>
    public sealed class RequiredLinkedObjectCompositeMissing : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public RequiredLinkedObjectCompositeMissing(IQuery query) : base(query, MessageTypeCode.RequiredLinkedObjectCompositeMissing)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              join fail in query.For<Order.RequiredLinkedObjectCompositeMissing>() on order.Id equals fail.OrderId
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
                                                                                              new XAttribute("name", query.For<Position>().Single(x => x.Id == fail.PositionId).Name)))),
                                      PeriodStart = order.BeginDistributionDate,
                                      PeriodEnd = order.EndDistributionDatePlan,
                                      ProjectId = order.ProjectId,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}