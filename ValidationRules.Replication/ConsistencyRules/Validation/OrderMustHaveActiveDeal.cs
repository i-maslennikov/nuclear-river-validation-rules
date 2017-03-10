using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, к которым не привязана или привязана неактивная или удалённая сделка, должна выводиться ошибка в релизном режиме, предупреждение в массовом и единичном
    /// "Для заказа указана неактивная работа"
    /// "Для заказа не указана работа"
    /// 
    /// Source: OrderDealRelationOrderValidationRule
    /// </summary>
    public sealed class OrderMustHaveActiveDeal : ValidationResultAccessorBase
    {
        public OrderMustHaveActiveDeal(IQuery query) : base(query, MessageTypeCode.OrderMustHaveActiveDeal)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from inactive in query.For<Order.InactiveReference>().Where(x => x.OrderId == order.Id).DefaultIfEmpty()
                from missing in query.For<Order.MissingRequiredField>().Where(x => x.OrderId == order.Id).DefaultIfEmpty()
                where inactive != null && inactive.Deal || missing != null && missing.Deal
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "state", missing != null && missing.Deal ? (int)DealState.Missing : inactive != null && inactive.Deal ? (int)DealState.Inactive : (int)DealState.NotSet } },
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
