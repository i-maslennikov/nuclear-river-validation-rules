using System;
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
    /// Для заказов, у которых не достаточно средств для выпуска, должна выводиться ошибка
    /// "Для оформления заказа недостаточно средств. Необходимо: {0}. Имеется: {1}. Необходим лимит: {2}"
    /// </summary>
    public sealed class AccountBalanceShouldBePositiveActor : IActor
    {
        private const int MessageTypeId = 14;

        // В erm эта проверка не вызывается при ручной проверке, только при сборке (в том числе бете)
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.None)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        private readonly ValidationRuleShared _validationRuleShared;

        public AccountBalanceShouldBePositiveActor(ValidationRuleShared validationRuleShared)
        {
            _validationRuleShared = validationRuleShared;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            return _validationRuleShared.ProcessRule(GetValidationResults, MessageTypeId);
        }

        private static IQueryable<Version.ValidationResult> GetValidationResults(IQuery query, long version)
        {
            // Ошибка выводится в городе назначения и городе источнике.
            var orderSourceProjects = query.For<Order>().Select(x => new { x.Id, x.AccountId, x.BeginDistributionDate, x.EndDistributionDate, ProjectId = x.SourceProjectId });
            var orderDestProjects = query.For<Order>().Select(x => new { x.Id, x.AccountId, x.BeginDistributionDate, x.EndDistributionDate, ProjectId = x.DestProjectId });

            var ruleResults = from accountPeriod in query.For<AccountPeriod>()
                              join order in orderSourceProjects.Union(orderDestProjects) on accountPeriod.AccountId equals order.AccountId
                              where order.BeginDistributionDate < accountPeriod.End && accountPeriod.Start < order.EndDistributionDate
                              where accountPeriod.Balance + accountPeriod.LimitAmount - accountPeriod.ReleaseAmount - (accountPeriod.OwerallLockedAmount - accountPeriod.LockedAmount)  < 0
                              select new Version.ValidationResult
                                  {
                                      MessageType = MessageTypeId,
                                      MessageParams = new XDocument(new XElement("account",
                                                                                 new XAttribute("id", accountPeriod.AccountId),
                                                                                 new XAttribute("available", accountPeriod.Balance - (accountPeriod.OwerallLockedAmount - accountPeriod.LockedAmount)),
                                                                                 new XAttribute("planned", accountPeriod.ReleaseAmount),
                                                                                 new XAttribute("required", accountPeriod.ReleaseAmount - (accountPeriod.Balance - (accountPeriod.OwerallLockedAmount - accountPeriod.LockedAmount))))),
                                      PeriodStart = accountPeriod.Start,
                                      PeriodEnd = accountPeriod.End,
                                      ProjectId = order.ProjectId,
                                      VersionId = version,

                                      ReferenceType = EntityTypeIds.Order,
                                      ReferenceId = order.Id,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
