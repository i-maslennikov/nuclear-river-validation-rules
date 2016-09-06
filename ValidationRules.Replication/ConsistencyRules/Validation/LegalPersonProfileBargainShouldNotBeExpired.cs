using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, у профиля (любого из) юрлица клиента которого указана дата договора меньшая чем дата подписания заказа, должно выводиться информационное сообщение.
    /// "У юр. лица клиента, в профиле {0} указан договор с датой окончания действия раньше даты подписания заказа"
    /// 
    /// Source: BargainEndDateOrderValidationRule
    /// </summary>
    public sealed class LegalPersonProfileBargainShouldNotBeExpired : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Info)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.None)
                                                                    .WhenMassRelease(Result.None);

        public LegalPersonProfileBargainShouldNotBeExpired(IQuery query) : base(query, 20)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from expired in query.For<Order.LegalPersonProfileBargainExpired>().Where(x => x.OrderId == order.Id)
                              select new Version.ValidationResult
                              {
                                  MessageParams = new XDocument(
                                          new XElement("root",
                                                       new XElement("legalPersonProfile",
                                                                    new XAttribute("id", expired.LegalPersonProfileId),
                                                                    new XAttribute("number", expired.LegalPersonProfileName)),
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
