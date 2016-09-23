using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Если для позиции заказа, такой что для любой её номенклатуры существует обязательный шаблон РМ, не существует объекта привязки с такой номенклатурой без указанного РМ, то должна выводиться ошибка:
    ///"В позиции {orderPosition} необходимо указать рекламные материалы" - для простой позиции
    ///"В позиции {orderPosition} необходимо указать рекламные материалы для подпозиции {position}" - для сложной позиции
    /// 
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/OrderCheckPositionMustHaveAdvertisements
    /// </summary>
    public sealed class RequiredAdvertisementMissing : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public RequiredAdvertisementMissing(IQuery query) : base(query, MessageTypeCode.RequiredAdvertisementMissing)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              join fail in query.For<Order.RequiredAdvertisementMissing>() on order.Id equals fail.OrderId
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(new XElement("root",
                                                                                 new XElement("order",
                                                                                              new XAttribute("id", order.Id),
                                                                                              new XAttribute("number", order.Number)),
                                                                                  new XElement("orderPosition",
                                                                                              new XAttribute("id", fail.OrderPositionId),
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
