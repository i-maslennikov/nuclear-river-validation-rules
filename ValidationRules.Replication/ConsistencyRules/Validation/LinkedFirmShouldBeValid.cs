using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, к которым привязана неактуальная фирма, должна выводиться ошибка.
    /// "Фирма {0} удалена"
    /// "Фирма {0} скрыта навсегда"
    /// "Фирма {0} скрыта до выяснения"
    /// 
    /// Source: FirmsOrderValidationRule
    /// </summary>
    public sealed class LinkedFirmShouldBeValid : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public LinkedFirmShouldBeValid(IQuery query) : base(query, MessageTypeCode.LinkedFirmShouldBeValid)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from firm in query.For<Order.InvalidFirm>().Where(x => x.OrderId == order.Id)
                              select new Version.ValidationResult
                              {
                                  MessageParams = new XDocument(
                                          new XElement("root",
                                                       new XElement("message",
                                                                    new XAttribute("state", firm.State)),
                                                       new XElement("firm",
                                                                    new XAttribute("id", firm.FirmId),
                                                                    new XAttribute("name", firm.FirmName)),
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
