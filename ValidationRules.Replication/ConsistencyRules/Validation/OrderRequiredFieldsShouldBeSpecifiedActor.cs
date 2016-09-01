using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, у которых не заполнено одно из одз полей, должна выводиться ошибка.
    /// "Необходимо заполнить все обязательные для заполнения поля: {0}"
    /// 
    /// Source: RequiredFieldsNotEmptyOrderValidationRule
    /// </summary>
    public sealed class OrderRequiredFieldsShouldBeSpecifiedActor : IActor
    {
        public const int MessageTypeId = 28;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        private readonly ValidationRuleShared _validationRuleShared;

        public OrderRequiredFieldsShouldBeSpecifiedActor(ValidationRuleShared validationRuleShared)
        {
            _validationRuleShared = validationRuleShared;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            return _validationRuleShared.ProcessRule(GetValidationResults, MessageTypeId);
        }

        private static IQueryable<Version.ValidationResult> GetValidationResults(IQuery query, long version)
        {
            var ruleResults = from order in query.For<Order>()
                              from missing in query.For<Order.MissingRequiredField>().Where(x => x.OrderId == order.Id)
                              select new Version.ValidationResult
                              {
                                  MessageType = MessageTypeId,
                                  MessageParams = new XDocument(
                                          new XElement("root",
                                                       new XElement("message",
                                                                    missing.Currency ? new XElement("currency") : null,
                                                                    missing.BranchOfficeOrganizationUnit ? new XElement("branchOfficeOrganizationUnit") : null,
                                                                    missing.Inspector ? new XElement("inspector") : null,
                                                                    missing.LegalPerson ? new XElement("legalPerson") : null,
                                                                    missing.LegalPersonProfile ? new XElement("legalPersonProfile") : null),
                                                       new XElement("order",
                                                                    new XAttribute("id", order.Id),
                                                                    new XAttribute("number", order.Number)))),
                                  PeriodStart = order.BeginDistribution,
                                  PeriodEnd = order.EndDistributionPlan,
                                  ProjectId = order.ProjectId,
                                  VersionId = version,

                                  Result = RuleResult,
                              };

            return ruleResults;
        }
    }
}
