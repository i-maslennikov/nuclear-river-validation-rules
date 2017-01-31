using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов с продажами в рубрику адреса, к которым привязана рубрика не принадлежащая адресу, должно выводиться предупреждение в единичной, ошибка в массовой.
    /// "В позиции {0} найдена рубрика {1}, не принадлежащая адресу {2}"
    /// 
    /// Source: LinkingObjectsOrderValidationRule
    /// </summary>
    public sealed class LinkedCategoryFirmAddressShouldBeValid : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public LinkedCategoryFirmAddressShouldBeValid(IQuery query) : base(query, MessageTypeCode.LinkedCategoryFirmAddressShouldBeValid)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from categoryFirmAddress in query.For<Order.InvalidCategoryFirmAddress>().Where(x => x.OrderId == order.Id)
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(
                                          new XElement("root",
                                              new XElement("message",
                                                  new XAttribute("state", categoryFirmAddress.State)),
                                              new XElement("firmAddress",
                                                  new XAttribute("id", categoryFirmAddress.FirmAddressId)),
                                              new XElement("category",
                                                  new XAttribute("id", categoryFirmAddress.CategoryId)),
                                              new XElement("order",
                                                  new XAttribute("id", order.Id)),
                                              new XElement("opa",
                                                new XElement("orderPosition", new XAttribute("id", categoryFirmAddress.OrderPositionId)),
                                                new XElement("position", new XAttribute("id", categoryFirmAddress.PositionId)))
                                              )),

                                      PeriodStart = order.BeginDistribution,
                                      PeriodEnd = order.EndDistributionPlan,
                                      OrderId = order.Id,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
