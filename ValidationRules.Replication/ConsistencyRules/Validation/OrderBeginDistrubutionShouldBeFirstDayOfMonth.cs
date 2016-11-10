using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, у которых дата начала размещения не является первым днём месяца, должна выводиться ошибка.
    /// "Указана некорректная дата начала размещения"
    /// 
    /// Source: DistributionDatesOrderValidationRule
    /// </summary>
    public sealed class OrderBeginDistrubutionShouldBeFirstDayOfMonth : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public OrderBeginDistrubutionShouldBeFirstDayOfMonth(IQuery query) : base(query, MessageTypeCode.OrderBeginDistrubutionShouldBeFirstDayOfMonth)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from date in query.For<Order.InvalidBeginDistributionDate>().Where(x => x.OrderId == order.Id)
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(
                                          new XElement("root",
                                              new XElement("order",
                                                  new XAttribute("id", order.Id),
                                                  new XAttribute("name", order.Number)))),

                                      PeriodStart = order.BeginDistribution,
                                      PeriodEnd = order.EndDistributionPlan,
                                      OrderId = order.Id,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
