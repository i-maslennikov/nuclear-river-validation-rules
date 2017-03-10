using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, у которых период размещения не совпадает с датами в счетах на оплату, должна выводиться ошибка
    /// "Период размещения, указанный в заказе и в счете не совпадают"
    /// 
    /// Source: BillsSumsOrderValidationRule
    /// </summary>
    public sealed class BillsPeriodShouldMatchOrder : ValidationResultAccessorBase
    {
        public BillsPeriodShouldMatchOrder(IQuery query) : base(query, MessageTypeCode.BillsPeriodShouldMatchOrder)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from date in query.For<Order.InvalidBillsPeriod>().Where(x => x.OrderId == order.Id)
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
