using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AccountRules.Validation
{
    /// <summary>
    /// Для заказов, у которых есть блокировки, должна выводиться ошибка.
    /// "Заказ {0} имеет созданную блокировку на указанный период"
    /// 
    /// Source: LockNoExistsOrderValidationRule/OrdersCheckOrderHasLock
    /// </summary>
    public sealed class LockShouldNotExist : ValidationResultAccessorBase
    {
        public LockShouldNotExist(IQuery query) : base(query, MessageTypeCode.LockShouldNotExist)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                join @lock in query.For<Order.Lock>() on order.Id equals @lock.OrderId
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrder>(order.Id))
                                .ToXDocument(),

                        PeriodStart = @lock.Start,
                        PeriodEnd = @lock.End,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
