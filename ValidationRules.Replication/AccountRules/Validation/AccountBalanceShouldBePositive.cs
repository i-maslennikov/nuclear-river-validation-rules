using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AccountRules.Validation
{
    /// <summary>
    /// Для заказов, у которых не достаточно средств для выпуска, должна выводиться ошибка
    /// "Для оформления заказа недостаточно средств. Необходимо: {0}. Имеется: {1}. Необходим лимит: {2}"
    /// 
    /// Проверка в erm допускает отрицательную сумму в 1 копейку (на самом деле основывается на настройке SignificantDigitsNumber)
    /// Source: BalanceOrderValidationRule/OrdersCheckOrderInsufficientFunds
    /// </summary>
    public sealed class AccountBalanceShouldBePositive : ValidationResultAccessorBase
    {
        // В erm эта проверка не вызывается при ручной проверке, только при сборке (в том числе бете)
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.None)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        // todo: завести настройку SignificantDigitsNumber и вообще решить вопрос с настройками проверок
        private static readonly decimal Epsilon = 0.01m;

        public AccountBalanceShouldBePositive(IQuery query) : base(query, MessageTypeCode.AccountBalanceShouldBePositive)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            // Ошибка выводится в городе назначения и городе источнике.
            // todo: посмотреть на решение в другом контексте, где в этой ситуации заведена таблица LinkedProject
            var orderSourceProjects = query.For<Order>().Select(x => new { x.Id, x.AccountId, x.Number, x.BeginDistributionDate, x.EndDistributionDate, ProjectId = x.SourceProjectId });
            var orderDestProjects = query.For<Order>().Select(x => new { x.Id, x.AccountId, x.Number, x.BeginDistributionDate, x.EndDistributionDate, ProjectId = x.DestProjectId });

            var ruleResults =
                from accountPeriod in query.For<AccountPeriod>()
                join order in orderSourceProjects.Union(orderDestProjects) on accountPeriod.AccountId equals order.AccountId
                where order.BeginDistributionDate < accountPeriod.End && accountPeriod.Start < order.EndDistributionDate
                where accountPeriod.Balance + accountPeriod.LimitAmount - accountPeriod.ReleaseAmount - (accountPeriod.OwerallLockedAmount - accountPeriod.LockedAmount) < -Epsilon
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(
                            new XElement("root",
                                         new XElement("message",
                                                      new XAttribute("available", accountPeriod.Balance - (accountPeriod.OwerallLockedAmount - accountPeriod.LockedAmount)),
                                                      new XAttribute("planned", accountPeriod.ReleaseAmount),
                                                      new XAttribute("required", accountPeriod.ReleaseAmount - (accountPeriod.Balance - (accountPeriod.OwerallLockedAmount - accountPeriod.LockedAmount)))),
                                         new XElement("account",
                                                      new XAttribute("id", accountPeriod.AccountId)),
                                         new XElement("order",
                                                      new XAttribute("id", order.Id),
                                                      new XAttribute("number", order.Number)))),
                        PeriodStart = accountPeriod.Start,
                        PeriodEnd = accountPeriod.End,
                        ProjectId = order.ProjectId,

                        Result = RuleResult,
                    };

            return ruleResults;
        }
    }
}
