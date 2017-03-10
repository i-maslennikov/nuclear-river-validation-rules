using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AccountRules.Validation
{
    /// <summary>
    /// Для заказов, у которых нет лицевого счёта, должна выводиться ошибка.
    /// "Заказ {0} не имеет привязки к лицевому счёту"
    /// 
    /// Source: AccountExistsOrderValidationRule
    /// </summary>
    public sealed class AccountShouldExist : ValidationResultAccessorBase
    {
        public AccountShouldExist(IQuery query) : base(query, MessageTypeCode.AccountShouldExist)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                where !order.AccountId.HasValue
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrder>(order.Id))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistributionDate,
                        PeriodEnd = order.EndDistributionDate,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
