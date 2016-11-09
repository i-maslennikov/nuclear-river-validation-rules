using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, которые связаны с неактиыми или удалёнными объектами BranchOfficeOrganizationUnit, BranchOffice, LegalPerson, LegalPersonProfile, должна выводиться ошибка
    /// "Заказ ссылается на неактивные объекты: {0}"
    /// 
    /// Source: OrderHasActiveLegalDetailsOrderValidationRule
    /// </summary>
    public sealed class OrderMustHaveActiveLegalEntities : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.None)
                                                                    .WhenMassRelease(Result.None);

        public OrderMustHaveActiveLegalEntities(IQuery query) : base(query, MessageTypeCode.OrderMustHaveActiveLegalEntities)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              from inactive in query.For<Order.InactiveReference>().Where(x => x.OrderId == order.Id)
                              where inactive.BranchOffice || inactive.BranchOfficeOrganizationUnit || inactive.LegalPerson || inactive.LegalPersonProfile
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(
                                          new XElement("root",
                                              new XElement("order",
                                                  new XAttribute("id", order.Id),
                                                  new XAttribute("number", order.Number)),
                                              new XElement("message",
                                                  inactive.BranchOfficeOrganizationUnit ? new XElement("branchOfficeOrganizationUnit") : null,
                                                  inactive.BranchOffice ? new XElement("branchOffice") : null,
                                                  inactive.LegalPerson ? new XElement("legalPerson") : null,
                                                  inactive.LegalPersonProfile ? new XElement("legalPersonProfile") : null))),

                                      PeriodStart = order.BeginDistribution,
                                      PeriodEnd = order.EndDistributionPlan,
                                      OrderId = order.Id,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
