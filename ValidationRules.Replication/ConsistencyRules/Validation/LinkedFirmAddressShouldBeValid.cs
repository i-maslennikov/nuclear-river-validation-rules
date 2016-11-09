using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, к которым привязан неактульный адрес, должна выводиться ошибка.
    /// "В позиции {0} адрес фирмы {1} скрыт навсегда"
    /// "В позиции {0} адрес фирмы {1} скрыт до выяснения"
    /// "В позиции {0} найден неактивный адрес {1}" -> "В позиции {0} адрес фирмы {1} неактивен"
    /// "В позиции {0} найден адрес {1}, не принадлежащий фирме заказа" -> "В позиции {0} адрес фирмы {1} не принадлежит фирме заказа"
    /// 
    /// Source: LinkingObjectsOrderValidationRule
    /// 
    /// Тут я позволил себе не выводить все сообщения, а только одно в следующем порядке: не принадлежит, навсегда, неактивен, до выяснения.
    /// </summary>
    public sealed class LinkedFirmAddressShouldBeValid : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public LinkedFirmAddressShouldBeValid(IQuery query) : base(query, MessageTypeCode.LinkedFirmAddressShouldBeValid)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from firmAddress in query.For<Order.InvalidFirmAddress>().Where(x => x.OrderId == order.Id)
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(
                                          new XElement("root",
                                              new XElement("message",
                                                  new XAttribute("invalidFirmAddressState", (int)firmAddress.State)),
                                              new XElement("firmAddress",
                                                  new XAttribute("id", firmAddress.FirmAddressId),
                                                  new XAttribute("name", firmAddress.FirmAddressName)),
                                              new XElement("order",
                                                  new XAttribute("id", order.Id),
                                                  new XAttribute("number", order.Number)),
                                              new XElement("orderPosition",
                                                  new XAttribute("id", firmAddress.OrderPositionId),
                                                  new XAttribute("name", firmAddress.OrderPositionName)))),

                                      PeriodStart = order.BeginDistribution,
                                      PeriodEnd = order.EndDistributionPlan,
                                      OrderId = order.Id,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
