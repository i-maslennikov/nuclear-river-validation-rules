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
    /// Для заказов, к которым привязана неактуальная фирма, должна выводиться ошибка.
    /// "Фирма {0} удалена"
    /// "Фирма {0} скрыта навсегда"
    /// "Фирма {0} скрыта до выяснения"
    /// 
    /// Source: FirmsOrderValidationRule
    /// </summary>
    public sealed class LinkedFirmShouldBeValidActor : IActor
    {
        public const int MessageTypeId = 17;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        private readonly ValidationRuleShared _validationRuleShared;

        public LinkedFirmShouldBeValidActor(ValidationRuleShared validationRuleShared)
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
                              from firm in query.For<Order.InvalidFirm>().Where(x => x.OrderId == order.Id)
                              select new Version.ValidationResult
                              {
                                  MessageType = MessageTypeId,
                                  MessageParams = new XDocument(
                                          new XElement("root",
                                                       new XElement("message",
                                                                    new XAttribute("state", firm.State)),
                                                       new XElement("firm",
                                                                    new XAttribute("id", firm.FirmId),
                                                                    new XAttribute("name", firm.Name)),
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
