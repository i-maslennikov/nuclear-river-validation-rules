using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

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
        // todo: завести настройку SignificantDigitsNumber и вообще решить вопрос с настройками проверок
        private static readonly decimal Epsilon = 0.01m;

        public AccountBalanceShouldBePositive(IQuery query) : base(query, MessageTypeCode.AccountBalanceShouldBePositive)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var nonFreeOfChargeOrders = query.For<Order>().Where(x => !x.IsFreeOfCharge);

            var ruleResults =
                from accountPeriod in query.For<Account.AccountPeriod>().Where(x => x.ReleaseAmount > 0)
                join order in nonFreeOfChargeOrders on accountPeriod.AccountId equals order.AccountId
                where order.BeginDistributionDate < accountPeriod.End && accountPeriod.Start < order.EndDistributionDate
                where accountPeriod.Balance - accountPeriod.ReleaseAmount - (accountPeriod.OwerallLockedAmount - accountPeriod.LockedAmount) <= -Epsilon
                where !query.For<Order.DebtPermission>().Any(x => x.OrderId == order.Id && x.Start <= accountPeriod.Start && accountPeriod.End <= x.End)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object>
                                        {
                                                { "available", accountPeriod.Balance - (accountPeriod.OwerallLockedAmount - accountPeriod.LockedAmount) },
                                                { "planned", accountPeriod.ReleaseAmount }
                                        },
                                    new Reference<EntityTypeAccount>(accountPeriod.AccountId),
                                    new Reference<EntityTypeOrder>(order.Id))
                                .ToXDocument(),

                        PeriodStart = accountPeriod.Start,
                        PeriodEnd = accountPeriod.End,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
