using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, у которых период размещения не совпадает с датами в счетах на оплату, должна выводиться ошибка
    /// "Сумма по счетам не совпадает с планируемой суммой заказа"
    /// 
    /// Source: BillsSumsOrderValidationRule
    /// </summary>
    public sealed class BillsSumShouldMatchOrder : ValidationResultAccessorBase
    {
        public BillsSumShouldMatchOrder(IQuery query) : base(query, MessageTypeCode.BillsSumShouldMatchOrder)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from date in query.For<Order.InvalidBillsTotal>().Where(x => x.OrderId == order.Id)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrder>(order.Id))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistribution,
                        PeriodEnd = order.EndDistributionPlan,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
