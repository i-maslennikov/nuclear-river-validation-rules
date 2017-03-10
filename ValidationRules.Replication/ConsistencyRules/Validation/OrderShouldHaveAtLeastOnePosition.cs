using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, без позиций, должна выводиться ошибка.
    /// "Заказ не содержит ни одной позиции"
    /// 
    /// Source: OrderHasAtLeastOnePositionOrderValidationRule
    /// </summary>
    public sealed class OrderShouldHaveAtLeastOnePosition : ValidationResultAccessorBase
    {
        public OrderShouldHaveAtLeastOnePosition(IQuery query) : base(query, MessageTypeCode.OrderShouldHaveAtLeastOnePosition)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from date in query.For<Order.HasNoAnyPosition>().Where(x => x.OrderId == order.Id)
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
