using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, к которым не привязана или привязана неактивная или удалённая сделка, должна выводиться ошибка в релизном режиме, предупреждение в массовом и единичном
    /// "Для заказа указана неактивная работа"
    /// "Для заказа не указана работа"
    /// 
    /// Source: OrderDealRelationOrderValidationRule
    /// </summary>
    public sealed class OrderMustHaveActiveDeal : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Warning)
                                                                    .WhenMassRelease(Result.Warning);

        public OrderMustHaveActiveDeal(IQuery query) : base(query, MessageTypeCode.OrderMustHaveActiveDeal)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from inactive in query.For<Order.InactiveReference>().Where(x => x.OrderId == order.Id).DefaultIfEmpty()
                              from missing in query.For<Order.MissingRequiredField>().Where(x => x.OrderId == order.Id).DefaultIfEmpty()
                              where inactive.Deal || missing.Deal
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(
                                          new XElement("root",
                                              new XElement("order",
                                                  new XAttribute("id", order.Id),
                                                  new XAttribute("number", order.Number)),
                                              new XElement("message",
                                                  new XAttribute("state", missing.Deal ? "missing" : "inactive")))),

                                      PeriodStart = order.BeginDistribution,
                                      PeriodEnd = order.EndDistributionPlan,
                                      OrderId = order.Id,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
