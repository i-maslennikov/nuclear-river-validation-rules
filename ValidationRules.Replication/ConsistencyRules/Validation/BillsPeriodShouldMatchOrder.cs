using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, у которых период размещения не совпадает с датами в счетах на оплату, должна выводиться ошибка
    /// "Период размещения, указанный в заказе и в счете не совпадают"
    /// 
    /// Source: BillsSumsOrderValidationRule
    /// </summary>
    public sealed class BillsPeriodShouldMatchOrder : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.None)
                                                                    .WhenMassRelease(Result.None);

        public BillsPeriodShouldMatchOrder(IQuery query) : base(query, MessageTypeCode.BillsPeriodShouldMatchOrder)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from date in query.For<Order.InvalidBillsPeriod>().Where(x => x.OrderId == order.Id)
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(
                                          new XElement("root",
                                              new XElement("order",
                                                  new XAttribute("id", order.Id)))),

                                      PeriodStart = order.BeginDistribution,
                                      PeriodEnd = order.EndDistributionPlan,
                                      OrderId = order.Id,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
