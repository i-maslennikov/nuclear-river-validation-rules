﻿using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, у которых период размещения не совпадает с датами в счетах на оплату, должна выводиться ошибка
    /// "Для заказа необходимо сформировать счета"
    /// 
    /// Source: BillsSumsOrderValidationRule
    /// </summary>
    public sealed class BillsShouldBeCreated : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.None)
                                                                    .WhenMassRelease(Result.None);

        public BillsShouldBeCreated(IQuery query) : base(query, MessageTypeCode.BillsShouldBeCreated)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from date in query.For<Order.MissingBills>().Where(x => x.OrderId == order.Id)
                              select new Version.ValidationResult
                              {
                                  MessageParams = new XDocument(
                                          new XElement("root",
                                                       new XElement("order",
                                                                    new XAttribute("id", order.Id),
                                                                    new XAttribute("number", order.Number)))),
                                  PeriodStart = order.BeginDistribution,
                                  PeriodEnd = order.EndDistributionPlan,
                                  ProjectId = order.ProjectId,

                                  Result = RuleResult,
                              };

            return ruleResults;
        }
    }
}