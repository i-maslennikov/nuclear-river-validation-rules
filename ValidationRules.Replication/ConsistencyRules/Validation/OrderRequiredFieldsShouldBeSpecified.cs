using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, у которых не заполнено одно из одз полей, должна выводиться ошибка.
    /// "Необходимо заполнить все обязательные для заполнения поля: {0}"
    /// 
    /// Source: RequiredFieldsNotEmptyOrderValidationRule
    /// </summary>
    public sealed class OrderRequiredFieldsShouldBeSpecified : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public OrderRequiredFieldsShouldBeSpecified(IQuery query) : base(query, MessageTypeCode.OrderRequiredFieldsShouldBeSpecified)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from missing in query.For<Order.MissingRequiredField>().Where(x => x.OrderId == order.Id)
                              where missing.Currency || missing.BranchOfficeOrganizationUnit || missing.Inspector
                                    || missing.LegalPerson || missing.LegalPersonProfile || missing.ReleaseCountPlan
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(
                                          new XElement("root",
                                              new XElement("message",
                                                  missing.Currency ? new XElement("currency") : null,
                                                  missing.BranchOfficeOrganizationUnit ? new XElement("branchOfficeOrganizationUnit") : null,
                                                  missing.Inspector ? new XElement("inspector") : null,
                                                  missing.LegalPerson ? new XElement("legalPerson") : null,
                                                  missing.LegalPersonProfile ? new XElement("legalPersonProfile") : null,
                                                  missing.ReleaseCountPlan ? new XElement("releaseCountPlan") : null),
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
