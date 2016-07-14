using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AccountRules.Validation
{
    /// <summary>
    /// Для заказов, у которых нет лицевого счёта, должна выводиться ошибка.
    /// "Заказ {0} не имеет привязки к лицевому счёту"
    /// 
    /// Source: AccountExistsOrderValidationRule
    /// </summary>
    public sealed class AccountShouldExistActor : IActor
    {
        public const int MessageTypeId = 12;

        // В erm эта проверка не вызывается при ручной проверке, только при сборке (в том числе бете)
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.None)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        private readonly ValidationRuleShared _validationRuleShared;

        public AccountShouldExistActor(ValidationRuleShared validationRuleShared)
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
                              where !order.AccountId.HasValue
                              select new Version.ValidationResult
                                  {
                                      MessageType = MessageTypeId,
                                      MessageParams = new XDocument(new XElement("root",
                                                                                 new XElement("order",
                                                                                              new XAttribute("id", order.Id),
                                                                                              new XAttribute("number", order.Number)))),
                                      PeriodStart = order.BeginDistributionDate,
                                      PeriodEnd = order.EndDistributionDate,
                                      ProjectId = order.DestProjectId,
                                      VersionId = version,

                                      ReferenceType = EntityTypeIds.Order,
                                      ReferenceId = order.Id,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
